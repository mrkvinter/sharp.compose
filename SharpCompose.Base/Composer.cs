using SharpCompose.Base.Composers;
using SharpCompose.Base.ElementBuilder;

namespace SharpCompose.Base;

public abstract class Composer
{
    public static readonly Composer Instance = new RenderTreeComposer();

    internal readonly Remembered Remembered = new();

    private readonly Stack<Scope> scopes = new();

    protected Scope? Root;

    public Scope? Current => scopes.TryPeek(out var parent) ? parent : null;

    public event Action? RecomposeEvent;

    internal static void Recompose()
    {
        Instance.RecomposeEvent?.Invoke();
    }

    public static void RootComposer(Action content)
    {
        var root = Instance.Root ?? Instance.CreateRoot();

        Instance.Remembered.ResetRememberedIndex();
        root.Clear();

        Instance.scopes.Push(Instance.Root!);
        content();
        Instance.scopes.Pop();
    }

    private Scope CreateRoot()
    {
        Root = new Scope();
        scopes.Clear();
        return Root;
    }

    public abstract void AddAttribute<T>(string name, T value);

    public void StartScope(Action<Composer> render, Action? childContent, IElementBuilder? meta = default)
    {
        var scope = new Scope
        {
            Factory = render,
            ElementBuilder = meta
        };

        if (!scopes.TryPeek(out var parent))
            parent?.AddChild(scope);

        scopes.Push(scope);
        Root ??= scope;
    }

    public void StopScope()
    {
        scopes.Pop();
    }

    public class Scope
    {
        private readonly List<Scope> child = new();

        public Action<Composer> Factory { get; init; } = default!;

        public IReadOnlyCollection<Scope> Child => child;

        public IElementBuilder? ElementBuilder { get; init; }

        public void Clear()
        {
            child.Clear();
        }

        public void AddChild(Scope scope)
        {
            child.Add(scope);
        }
    }
}