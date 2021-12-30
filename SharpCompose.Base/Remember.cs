namespace SharpCompose.Base;

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

    private static async Task<ValueRemembered<T>> GetAsync<T>(Func<Task<T>> creator)
    {
        var current = Composer.Instance.Current!;
        if (current.Remembered.TryGetNextRemembered<T>(out var result))
        {
            return result;
        }

        var value = await creator();

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

    public static async void LaunchedEffect(Func<Task> action)
    {
        var _ = await GetAsync<Unit>(async () =>
        {
            await action();
            // Composer.Instance.DeferredActions.Add(action);

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

internal struct Unit
{
}