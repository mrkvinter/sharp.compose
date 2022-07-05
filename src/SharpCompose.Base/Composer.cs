using SharpCompose.Base.Layouting;
using SharpCompose.Base.Modifiers;
using SharpCompose.Drawer.Core;
using BaseCompose = SharpCompose.Base.ComposesApi.BaseCompose;

namespace SharpCompose.Base;

public delegate MeasureResult Measure(Measurable[] measures, Constraints constraints);

public class Composer
{
    public static Composer Instance { get; } = new();

    private LayoutNode? root;
    public ICanvas Canvas { get; protected set; } = null!;

    internal readonly Stack<INode> Scopes = new(Array.Empty<INode>());

    protected internal LayoutNode Root => root ??= Instance.CreateRoot();

    internal INode? Current => Scopes.TryPeek(out var parent) ? parent : null;

    public bool RecomposingAsk { get; private set; }
    public bool Composing { get; private set; }

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
        {
            Instance.Root.SaveUnused();
            Instance.Root.Changed = false;
        }

        // We already have root scope, so we shouldn't run StartScope, because we will not have access to remember table
        Instance.Scopes.Push(Instance.Root);
        BaseCompose.CompositionLocalProvider(new[]
        {
            BaseCompose.LocalInputHandler.Provide(inputHandler)
        }, content);
        Instance.StopScope();
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

    private LayoutNode CreateRoot()
    {
        var createdRoot = new LayoutNode(IModifier.Empty,
            Canvas.StartGraphics())
        {
            Name = "0",
            Measure = BoxLayout.Measure(new BiasAlignment(-1, -1)),
        };

        Scopes.Clear();

        return createdRoot;
    }

    public void StartGroup()
    {
        INode Creator()
        {
            var createdScope = new GroupNode
            {
                Parent = Current!
            };
            Current!.AddChild(createdScope);

            return createdScope;
        }

        var scope = Remember.Get(Creator);
        Scopes.Peek().UnusedChildren.Remove(scope);

        scope.SaveUnused();

        Scopes.Push(scope);
    }

    public void EndGroup()
    {
        var scope = Scopes.Pop();
        foreach (var unusedChild in scope.UnusedChildren)
            unusedChild.Nodes.ForEach(e => e.Clear());
    }

    public void StartScope(IModifier modifier, Measure measure)
    {
        LayoutNode Creator()
        {
            var createdScope = new LayoutNode(modifier, Canvas.StartGraphics())
            {
                Name = string.Join(".", modifier.FromInToOut().OfType<DebugModifier>().Select(e => e.ScopeName)),
                Parent = Current!
            };
            Current!.AddChild(createdScope);

            return createdScope;
        }

        var scope = Remember.Get(Creator);
        scope.Measure = measure;
        Scopes.Peek().UnusedChildren.Remove(scope);

        scope.SaveUnused();

        if (scope.Changed)
        {
            scope.Update(modifier);
            scope.Changed = false;
        }

        Scopes.Push(scope);
    }

    public void StopScope()
    {
        var scope = Scopes.Pop();
        foreach (var unusedChild in scope.UnusedChildren)
            unusedChild.Clear();
    }
}