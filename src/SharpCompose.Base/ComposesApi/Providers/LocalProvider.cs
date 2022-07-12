using SharpCompose.Base.Nodes;

namespace SharpCompose.Base.ComposesApi.Providers;

public record Provider(Action StartProvide);

public abstract class LocalProvider
{
    protected static int CountProviders { get; set; }
}
public class LocalProvider<T> : LocalProvider
{
    private readonly T defaultValue;

    public T Value
    {
        get
        {
            var group = Composer.Instance.CurrentGroup;
            while (group != null)
            {
                if (group.Locals.TryGetValue(id, out var result))
                    return (T) result;

                var parent = group.Parent;
                while (parent != null && parent is not IGroupNode)
                {
                    parent = parent.Parent;
                }
                group = parent as IGroupNode;
            }
            
            return defaultValue;
        }
    }

    private readonly int id;

    public LocalProvider(T defaultValue)
    {
        this.defaultValue = defaultValue;
        id = CountProviders++;
    }

    public Provider Provide(T newValue)
    {
        return new Provider(() => Composer.Instance.CurrentGroup.Locals[id] = newValue!);
    }
}