using Microsoft.Extensions.Logging;
using SharpCompose.Base.Composers;
using SharpCompose.Base.ElementBuilder;
using SharpCompose.Base.Modifiers;

namespace SharpCompose.Base;

public abstract class Composer
{
    public static Composer Instance { get; set; } = new RenderTreeComposer();

    public static ILogger? Logger { get; set; }

    private readonly Stack<Scope> scopes = new();

    protected Scope? Root;

    public Scope? Current => scopes.TryPeek(out var parent) ? parent : null;

    public event Action? RecomposeEvent;

    public bool Composing { get; private set; }

    internal static void Recompose()
    {
        // Logger?.Log(LogLevel.Warning, "recompose was called");
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
        Root = new Scope(EmptyElementBuilder.Instance)
        {
            Name = "0"
        };
        scopes.Clear();
        return Root;
    }

    public abstract void BuildAttributes(IReadOnlyDictionary<string, object> attributes);

    public void StartScope(Action<Composer> attributeBuilder, IComponentModifier? componentModifier,
        IElementBuilder elementBuilder)
    {
        Scope Creator()
        {
            var createdScope = new Scope(elementBuilder)
            {
                AttributeBuilder = attributeBuilder,
            };

            if (scopes.TryPeek(out var parent))
            {
                parent.AddChild(createdScope);
                createdScope.Name = parent.Name + $"-{parent.Child.Count - 1}";
            }

            return createdScope;
        }

        var scope = Remember.Remember.Get(Creator).Value;

        if (scope.Changed)
        {
            scope.Clear();
            scope.Changed = false;
        }

        componentModifier?.MetaProducer?.Invoke(scope);
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
        private readonly Dictionary<string, object> meta = new();

        public Scope(IElementBuilder elementBuilder)
        {
            ElementBuilder = elementBuilder;
        }

        public Action<Composer> AttributeBuilder { get; init; } = default!;

        public IReadOnlyCollection<Scope> Child => child;

        public IElementBuilder ElementBuilder { get; }

        public readonly Remembered Remembered = new();

        public readonly Remembered RememberedSavable = new();

        public string Name { get; internal set; } = "";

        public bool Changed { get; set; }

        public void Clear()
        {
            child.Clear();
            meta.Clear();

            foreach (var value in Remembered.RememberedValues)
            {
                if (value is Remember.Remember.DisposableEffect disposableEffect)
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

        public void AddMeta<T>(T metaValue)
        {
            meta[typeof(T).Name] = metaValue!;
        }

        public void AddMeta<T>(string key, T metaValue)
        {
            meta[key] = metaValue!;
        }

        public bool TryGetMeta<T>(out T? result)
        {
            result = default;
            if (meta.TryGetValue(typeof(T).Name, out var r))
            {
                result = (T) r;

                return true;
            }

            return false;
        }

        public T? GetMeta<T>()
            => meta.TryGetValue(typeof(T).Name, out var r) ? (T?) r : default;

        public T? GetMetaByKey<T>(string key) => meta.TryGetValue(key, out var r) ? (T?) r : default;

        public override string ToString() => $"{nameof(Scope)} ({Name}) [{ElementBuilder.ToString()}]";
    }
}