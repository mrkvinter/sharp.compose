using System.Diagnostics;
using System.Reflection;
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
    internal bool WaitingInsertGroup => waitingInsertGroup;

    public int CountChangedNodes { get; private set; }

    private bool waitingInsertGroup;
    private long nodeCounter;

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

    [RootComposableApi]
    public static void RecomposeTree()
    {
        Instance.Composing = true;
        while (Instance.RecomposingAsk)
        {
            Instance.RecomposingAsk = false;
            var changedNodes = new List<IGroupNode>();
            var nodesToInvestigate = new Stack<IGroupNode>();
            nodesToInvestigate.Push(Instance.Root);
            while (nodesToInvestigate.Count > 0)
            {
                var node = nodesToInvestigate.Pop();
                if (node.Changed)
                {
                    changedNodes.Add(node);
                }

                foreach (var child in node.Children)
                {
                    switch (child)
                    {
                        case IGroupNode groupNode:
                            nodesToInvestigate.Push(groupNode);
                            break;
                        case IUINode uiNode:
                            nodesToInvestigate.Push(uiNode.GroupNode);
                            break;
                    }
                }
            }

            Instance.CountChangedNodes = changedNodes.Count;
            foreach (var changedNode in changedNodes)
            {
                if (!changedNode.Changed)
                    continue;

                Instance.Groups.Push(changedNode);
                changedNode.Changed = false;
                changedNode.SaveUnused();
                changedNode.Content?.Invoke();
                if (changedNode.Content != null)
                    Instance.EndGroup();
                else
                    Instance.Groups.Pop();
            }
        }

        Instance.Composing = false;
    }

    public static void Layout()
    {
        if (Instance.Composing)
            return;

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
        var createdRoot = new RootNode(Canvas.StartGraphics(), nodeCounter++)
        {
            Name = "Root",
            Measure = BoxLayout.Measure(new BiasAlignment(-1, -1))
        };

        UINodes.Clear();

        return createdRoot;
    }

    [GroupRootComposableApi]
    public void StartGroup(Action? content, bool changed = true)
    {
        [ComposableApi]
        IGroupNode Creator()
        {
            var createdScope = new GroupNode
            {
                Parent = waitingInsertGroup ? CurrentUINode : CurrentGroup,
                Id = nodeCounter++,
                Changed = changed,
                Content = content,
                Locals = CurrentGroup.Locals,
                HasExternalState = HasExternalState()
            };
            if (waitingInsertGroup)
                CurrentUINode.GroupNode = createdScope;
            else
                CurrentGroup.AddChild(createdScope);

            return createdScope;
        }


        var groupNode = Remember.Get(Creator);

        waitingInsertGroup = false;

        if (groupNode.Parent is IGroupNode groupNodeParent)
        {
            groupNodeParent.UnusedChildren.Remove(groupNode);
        }

        Groups.Push(groupNode);
        
        if (groupNode.Changed)
        {
            groupNode.Changed = false;
            groupNode.SaveUnused();
            groupNode.Content = content;
            content?.Invoke();
        }
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
        
        foreach (var key in groupNode.UnusedRememberedKeys)
        {
            if (groupNode.Remembered
                .TryGetNextRemembered<IRememberObserver>(key, out var result))
                result.OnForgotten();

            groupNode.Remembered.RemoveRemembered(key);
        }
    }

    [GroupRootComposableApi]
    public void StartNode(IModifier modifier, Measure measure)
    {
        LayoutNode Creator()
        {
            var createdScope = new LayoutNode(modifier, Canvas.StartGraphics())
            {
                Name = string.Join(".", modifier.FromInToOut().OfType<DebugModifier>().Select(e => e.ScopeName)),
                Parent = CurrentGroup,
            };
            CurrentGroup.AddChild(createdScope);

            return createdScope;
        }

        if (waitingInsertGroup)
            throw new InvalidOperationException(
                $"Imposable inserting node while waiting inserting group. Please, call {nameof(StartGroup)} before.");

        waitingInsertGroup = true;
        var layoutNode = Remember.Get(Creator);
        layoutNode.Measure = measure;
        CurrentGroup.UnusedChildren.Remove(layoutNode);
        layoutNode.Update(modifier);

        UINodes.Push(layoutNode);
    }

    public void EndNode()
    {
        UINodes.Pop();
    }

    /// <summary>
    /// HasExternalState:
    /// - if we found method with ComposableAttribute and it has empty parameters, then it has no external state
    /// - if we found lambda with no captured variables, then it has no external state
    /// - if we found lambda with captured variables, then it has external state
    /// </summary>
    /// <returns>
    ///     <see langword="true"/> if current node has external state, otherwise <see langword="false"/>
    /// </returns>
    private bool HasExternalState()
    {
        var stackTrace = new StackTrace();
        for (int i = 1; i < stackTrace.FrameCount; i++)
        {
            var frame = stackTrace.GetFrame(i)!;
            var method = frame.GetMethod()!;

            if (method.GetCustomAttribute<ComposableApiAttribute>() != null)
            {
                continue;
            }

            if (method.GetCustomAttribute<ComposableAttribute>() != null)
            {
                return method.GetParameters().Length != 0;
            }

            // I don't now better way to check if lambda has captured variables
            // Closure classes have name like "<>c__DisplayClass0_0"
            if (method.DeclaringType!.Name.StartsWith("<>c__DisplayClass", StringComparison.Ordinal))
            {
                return true;
            }

            // No closure classes have name like "<>c."
            if (method.DeclaringType!.Name.StartsWith("<>c.", StringComparison.Ordinal))
            {
                return false;
            }
        }
        
        return true;
    }
}