using SharpCompose.Base.Layouting;
using SharpCompose.Base.Modifiers;
using SharpCompose.Base.Nodes;
using SharpCompose.Drawer.Core;
using BaseCompose = SharpCompose.Base.ComposesApi.BaseCompose;

namespace SharpCompose.Base;

public delegate MeasureResult Measure(Measurable[] measures, Constraints constraints);

public class Composer
{
    public static Composer Instance { get; } = new();

    private RootNode? root;
    public ICanvas Canvas { get; protected set; } = null!;

    internal readonly Stack<IUINode> UINodes = new(Array.Empty<IUINode>());
    internal readonly Stack<IGroupNode> Groups = new(Array.Empty<IGroupNode>());

    protected internal RootNode Root => root ??= Instance.CreateRoot();

    internal IUINode CurrentUINode => UINodes.Peek();
    internal IGroupNode CurrentGroup => Groups.Peek();

    internal IGroupNode? PossibleCurrentGroup => Groups.TryPeek(out var groupNode) ? groupNode : null;

    public bool RecomposingAsk { get; private set; }
    public bool Composing { get; private set; }

    private bool waitingInsertGroup;

    //todo: make internal again
    public static void Recompose()
    {
        Instance.RecomposingAsk = true;
    }

    public void Init(ICanvas canvas)
    {
        Canvas = canvas;
    }

    internal void ForceClear()
    {
        root = null;
    }

    [RootComposableApi]
    public static void Compose(IInputHandler inputHandler, Action content)
    {
        Instance.Composing = true;
        if (Instance.Root.Changed)
            Instance.Root.Changed = false;

        // We already have root scope, so we shouldn't run StartScope, because we will not have access to remember table
        Instance.Groups.Push(Instance.Root);
        Instance.UINodes.Push(Instance.Root);

        BaseCompose.CompositionLocalProvider(new[]
        {
            BaseCompose.LocalInputHandler.Provide(inputHandler)
        }, content);

        Instance.EndNode();
        Instance.EndGroup();

        Instance.Composing = false;
    }

    public static void Layout()
    {
        if (Instance.Composing)
            return;

        Instance.RecomposingAsk = false;
        var (width, height) = Instance.Canvas.Size;

        Instance.Root.ClearGraphics();
        Instance.Root
            .Measurable.Measure(new Constraints(0, width, 0, height))
            .Placeable(0, 0);
    }

    public static void Draw()
    {
        if (Instance.Composing)
            return;

        Instance.Root.DrawNode(Instance.Canvas);
        Instance.Canvas.Draw();
    }

    private RootNode CreateRoot()
    {
        var createdRoot = new RootNode(Canvas.StartGraphics())
        {
            Name = "Root",
            Measure = BoxLayout.Measure(new BiasAlignment(-1, -1)),
        };

        UINodes.Clear();

        return createdRoot;
    }

    [ComposableApi]
    public void StartGroup()
    {
        [ComposableApi]
        IGroupNode Creator()
        {
            var createdScope = new GroupNode
            {
                Parent = waitingInsertGroup ? CurrentUINode : CurrentGroup
            };
            if (waitingInsertGroup)
                CurrentUINode.GroupNode = createdScope;
            else
                CurrentGroup.AddChild(createdScope);

            return createdScope;
        }


        var key = ComposeKey.GetKey();
        var loopPostfix = string.Empty;
        if (CurrentGroup.CountNodes.TryGetValue(key, out var count))
        {
            loopPostfix += count;
            CurrentGroup.CountNodes[key]++;
        }
        else
        {
            CurrentGroup.CountNodes.Add(key, 0);
        }

        var groupNode = Remember.GetInternal(loopPostfix, Creator);

        waitingInsertGroup = false;

        if (groupNode.Parent is IGroupNode groupNodeParent)
            groupNodeParent.UnusedChildren.Remove(groupNode);


        groupNode.SaveUnused();

        Groups.Push(groupNode);
    }

    public void EndGroup()
    {
        var groupNode = Groups.Pop();
        groupNode.CountNodes.Clear();
        foreach (var unusedChild in groupNode.UnusedChildren)
        {
            groupNode.RemoveChild(unusedChild);
            unusedChild.Clear();
        }
    }

    [ComposableApi]
    public void StartNode(IModifier modifier, Measure measure)
    {
        LayoutNode Creator()
        {
            var createdScope = new LayoutNode(modifier, Canvas.StartGraphics())
            {
                Name = string.Join(".", modifier.FromInToOut().OfType<DebugModifier>().Select(e => e.ScopeName)),
                Parent = CurrentGroup
            };
            CurrentGroup.AddChild(createdScope);

            return createdScope;
        }

        if (waitingInsertGroup)
            throw new InvalidOperationException(
                $"Imposable inserting node while waiting inserting group. Please, call {nameof(StartGroup)} before.");

        waitingInsertGroup = true;
        var key = ComposeKey.GetKey();
        var loopPostfix = string.Empty;
        if (CurrentGroup.CountNodes.TryGetValue(key, out var count))
        {
            loopPostfix += count;
            CurrentGroup.CountNodes[key]++;
        }
        else
        {
            CurrentGroup.CountNodes.Add(key, 0);
        }

        var layoutNode = Remember.GetInternal(loopPostfix, Creator);
        layoutNode.Measure = measure;
        CurrentGroup.UnusedChildren.Remove(layoutNode);

        if (layoutNode.Changed)
        {
            layoutNode.Update(modifier);
            layoutNode.Changed = false;
        }

        UINodes.Push(layoutNode);
    }

    public void EndNode()
    {
        UINodes.Pop();
    }
}