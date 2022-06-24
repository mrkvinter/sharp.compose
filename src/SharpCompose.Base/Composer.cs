using SharpCompose.Base.ComposesApi.Providers;
using SharpCompose.Base.Layouting;
using SharpCompose.Base.Modifiers;
using SharpCompose.Base.Modifiers.DrawableModifiers;
using SharpCompose.Base.Modifiers.LayoutModifiers;
using SharpCompose.Drawer.Core;
using BaseCompose = SharpCompose.Base.ComposesApi.BaseCompose;

namespace SharpCompose.Base;

public delegate MeasureResult Measure(Measurable[] measures, Constraints constraints);

public class Composer
{
    public static Composer Instance { get; internal set; } = new();

    private Scope? root;
    public ICanvas Canvas { get; protected set; } = null!;

    private readonly Stack<Scope> scopes = new(Array.Empty<Scope>());

    protected internal Scope Root => root ??= Instance.CreateRoot();

    internal Scope? Current => scopes.TryPeek(out var parent) ? parent : null;

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

    [RootComposable]
    public static void Compose(IInputHandler inputHandler, Action content)
    {
        Instance.Composing = true;
        if (Instance.Root.Changed)
        {
            Instance.Root.SaveUnused();
            Instance.Root.Changed = false;
        }

        // We already have root scope, so we shouldn't run StartScope, because we will not have access to remember table
        Instance.scopes.Push(Instance.Root);
        BaseCompose.CompositionLocalProvider(new[]
        {
            LocalProviders.InputHandler.Provide(inputHandler)
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

    private Scope CreateRoot()
    {
        var createdRoot = new Scope(IModifier.Empty, BoxLayout.Measure(new BiasAlignment(-1, -1)),
            Canvas.StartGraphics())
        {
            Name = "0"
        };
        scopes.Clear();

        return createdRoot;
    }

    public void StartScope(IModifier modifier, Measure measure)
    {
        Scope Creator()
        {
            var createdScope = new Scope(modifier, measure, Canvas.StartGraphics());

            if (scopes.TryPeek(out var parent))
            {
                parent.AddChild(createdScope);
                createdScope.Name = parent.Name + $"-{parent.Children.Count - 1}";
            }

            return createdScope;
        }

        var scope = Remember.Get(Creator).Value;
        scopes.Peek().UnusedChildren.Remove(scope);

        if (scope.Changed)
        {
            scope.SaveUnused();
            scope.Update(modifier);
            scope.Changed = false;
        }

        scopes.Push(scope);
    }

    public void StopScope()
    {
        var scope = scopes.Pop();
        foreach (var unusedChild in scope.UnusedChildren)
            unusedChild.Clear();
    }

    public class Scope
        // protected internal class Scope
    {
        internal IModifier Modifier => modifier;

        internal readonly List<Scope> UnusedChildren = new();

        private readonly List<Scope> children = new();
        private readonly Measure measure;
        private readonly IGraphics graphics;
        private IModifier modifier;

        public Scope(IModifier modifier, Measure measure, IGraphics graphics)
        {
            this.modifier = modifier;
            this.measure = measure;
            this.graphics = graphics;
            Measurable = GetMeasure();
        }

        public IReadOnlyCollection<Scope> Children => children;

        public void Update(IModifier modifier)
        {
            this.modifier = modifier;
            Measurable = GetMeasure();
        }

        public readonly Remembered Remembered = new();

        public string Name { get; internal set; } = "";

        public bool Changed { get; set; }

        public Measurable Measurable { get; private set; }

        private Measurable GetMeasure()
        {
            var measurable = new Measurable
            {
                Measure = constraints =>
                {
                    graphics.Clear();
                    return measure(children
                        .Select(e => e.Measurable).ToArray(), constraints);
                }
            };

            var modifiers = modifier.SqueezeModifiers();

            foreach (var m in modifiers)
            {
                if (m is DebugModifier debugModifier)
                {
                    if (debugModifier.ScopeName != null) this.Name = debugModifier.ScopeName;
                    continue;
                }

                measurable = m switch
                {
                    ILayoutModifier layoutModifier => layoutModifier.Introduce(measurable),
                    IDrawableLayerModifier drawableModifier => drawableModifier.Introduce(measurable, graphics),
                    IParentDataModifier parentDataModifier => parentDataModifier.Introduce(measurable),
                    _ => measurable
                };
            }

            return measurable;
        }

        public void DrawNode(ICanvas canvas)
        {
            canvas.DrawGraphics(0, 0, graphics);
            children.ForEach(c => c.DrawNode(canvas));
        }

        public void SaveUnused()
        {
            UnusedChildren.Clear();
            UnusedChildren.AddRange(children);
        }

        public void Clear()
        {
            children.ForEach(c => c.Clear());
            children.Clear();

            foreach (var value in Remembered.RememberedValues)
            {
                if (value is Remember.DisposableEffect disposableEffect)
                {
                    disposableEffect.Dispose();
                }
            }

            Remembered.Clear();
        }

        public void AddChild(Scope scope)
        {
            children.Add(scope);
        }

        public void RemoveChild(Scope scope)
        {
            children.Remove(scope);
        }

        public override string ToString() =>
            $"{nameof(Scope)} ({Name}) [{Remembered.RememberedValues.FirstOrDefault()}]";
    }
}