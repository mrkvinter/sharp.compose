using SharpCompose.Base.Extensions;

namespace SharpCompose.Base.ComposesApi.Providers;

public record Provider(Action StartProvide);

public abstract class LocalProvider
{
    protected static int CountProviders { get; set; }
}
public class LocalProvider<T> : LocalProvider
    where T : IEquatable<T>
{
    private readonly T defaultValue;

    public T Value
    {
        get
        {
            var group = Composer.Instance.CurrentGroup;
            
            if (group.Locals.TryGetValue(id, out var result))
                return ((MutableState<T>) result).Value;
            
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
        return new Provider(() =>
        {
            var state = Remember.Get(() =>
            {
                var state = newValue.AsMutableState();
                Composer.Instance.CurrentGroup.Locals[id] = state;
                return state;
            });
            state.Value = newValue;
        });
    }
}