namespace SharpCompose.Base.Remember;

public static class RememberSavable
{
    public static ValueRemembered<T> Get<T>(Func<T> creator)
    {
        var current = Composer.Instance.Current!;
        if (current.RememberedSavable.TryGetNextRemembered<T>(out var result))
        {
            return result;
        }

        var value = creator();

        return current.RememberedSavable.AddRemembered(value);
    }

    public static ValueRemembered<T> Get<T>(T value) where T : struct
    {
        return Get(() => value);
    }
}