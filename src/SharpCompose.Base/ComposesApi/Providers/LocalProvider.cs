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
            var node = Composer.Instance.Current!;
            while (node != null)
            {
                if (node.Locals.TryGetValue(id, out var result))
                    return (T) result;

                node = node.Parent;
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
        return new Provider(() => Composer.Instance.Current!.Locals[id] = newValue!);
    }
}