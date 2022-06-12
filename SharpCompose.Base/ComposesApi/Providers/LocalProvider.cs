using System.Drawing;

namespace SharpCompose.Base.ComposesApi.Providers;

public record struct Provider(Action StartProvide, Action EndProvide);

public class LocalProvider<T>
{
    public T Value { get; protected set; }

    public LocalProvider(T defaultValue)
    {
        Value = defaultValue;
    }

    public Provider Provide(T newValue)
    {
        var oldValue = Value;
        return new Provider(() => Value = newValue, () => Value = oldValue);
    }
}