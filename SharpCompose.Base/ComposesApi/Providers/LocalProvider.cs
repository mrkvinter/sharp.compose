namespace SharpCompose.Base.ComposesApi.Providers;

public record struct Provider(Action StartProvide, Action EndProvide);

public class LocalProvider<T>
{
    private static LocalProvider<T> Instance { get; } = new();

    public static T Value => Instance.value;

    private T value;

    public static Provider Provide(T newValue)
    {
        var oldValue = Instance.value;
        return new Provider(() => Instance.value = newValue, () => Instance.value = oldValue);
    }
}