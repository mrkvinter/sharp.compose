namespace SharpCompose.Base.Remember;

public static class Remember
{
    public static ValueRemembered<T> Get<T>(Func<T> creator)
    {
        var current = Composer.Instance.Current!;
        if (current.Remembered.TryGetNextRemembered<T>(out var result))
        {
            return result;
        }

        var value = creator();

        return current.Remembered.AddRemembered(value);
    }

    public static ValueRemembered<T> Get<T>(T value) where T : struct
    {
        return Get(() => value);
    }

    public static void LaunchedEffect(Action action)
    {
        var _ = Get<Unit>(() =>
        {
            action();

            return default;
        });
    }

    public static void LaunchedEffect(Func<Task> action)
    {
        var _ = Get<Unit>(() =>
        {
            action();

            return default;
        });
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