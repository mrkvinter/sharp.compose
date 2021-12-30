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

    public static void DisposeEffect(Action action)
    {
        var _ = Get(() => new DisposableEffect(action));
    }

    internal class DisposableEffect : IDisposable
    {
        private readonly Action disposable;

        public DisposableEffect(Action disposable)
        {
            this.disposable = disposable;
        }

        public void Dispose()
        {
            disposable.Invoke();
        }
    }
}
}