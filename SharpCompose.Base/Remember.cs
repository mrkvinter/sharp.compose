namespace SharpCompose.Base;

public static class Remember
{
    public static Remembered.ValueRemembered<T> Get<T>(Func<T> creator)
    {
        if (Composer.Instance.Remembered.HasNextRemembered<T>())
        {
            return Composer.Instance.Remembered.NextRemembered<T>();
        }

        var value = creator();

        return Composer.Instance.Remembered.AddRemembered(value);
    }

    public static Remembered.ValueRemembered<T> Get<T>(T value) where T : struct
    {
        return Get(() => value);
    }
}