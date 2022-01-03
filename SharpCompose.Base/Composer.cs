using SharpCompose.Base.Composers;
using SharpCompose.Base.ElementBuilder;

namespace SharpCompose.Base;

public abstract class Composer
{
    public static Composer Instance = new RenderTreeComposer();

    private readonly Stack<Scope> scopes = new();

    protected Scope? Root;

    public Scope? Current => scopes.TryPeek(out var parent) ? parent : null;

    public event Action? RecomposeEvent;

    public bool Composing { get; private set; }

    internal List<Func<Task>> DeferredActions = new();

    internal static void Recompose()
    {
        Instance.RecomposeEvent?.Invoke();
    }

    public static void RootComposer(Action content)
    {
        Instance.Composing = true;
        var root = Instance.Root ?? Instance.CreateRoot();

        root.Remembered.ResetRememberedIndex();

        Instance.scopes.Push(root);
        content();
        Instance.scopes.Pop();
        Instance.Composing = false;
    }

    private Scope CreateRoot()
    {
        Root = new Scope
        {
            Name = "0"
        };
        scopes.Clear();
        return Root;
    }

    public abstract void BuildAttributes(IReadOnlyDictionary<string, object> attributes);

    public void StartScope(Action<Composer> attributeBuilder, IElementBuilder? meta = default)
    {
        Scope Creator()
        {
            var createdScope = new Scope
            {
                AttributeBuilder = attributeBuilder,
                ElementBuilder = meta
            };

            if (scopes.TryPeek(out var parent))
            {
                parent.AddChild(createdScope);
                createdScope.Name = parent.Name + $"-{parent.Child.Count - 1}";
            }

            return createdScope;
        }

        var scope = Remember.Get(Creator).Value!;

        if (scope.Changed)
        {
            scope.Clear();
            scope.Changed = false;
        }

        scope.Remembered.ResetRememberedIndex();

        scopes.Push(scope);
    }

    public void StopScope()
    {
        scopes.Pop();
    }

    public class Scope
    {
        private readonly List<Scope> child = new();

        public Action<Composer> AttributeBuilder { get; init; } = default!;

        public IReadOnlyCollection<Scope> Child => child;

        public IElementBuilder? ElementBuilder { get; init; }

        public readonly Remembered Remembered = new();

        public Remembered RememberedSavable = new();

        public string Name { get; internal set; } = "";
        public bool Changed { get; set; }

        public void Clear()
        {
            child.Clear();

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
            child.Add(scope);
        }

        public void RemoveChild(Scope scope)
        {
            child.Remove(scope);
        }

        public override string ToString()
        {
            var elementBuilderString = ElementBuilder?.ToString() ?? "null";
            return $"{nameof(Scope)} ({Name}) [{elementBuilderString}]";
        }
    }
}