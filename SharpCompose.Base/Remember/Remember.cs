using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace SharpCompose.Base;

public class RememberEnumerable<T> : IEnumerable<T>
{
    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public static class Remember
{
    private static int loopIndex;

    public class LoopIndexController : IDisposable
    {
        private readonly int oldLoopIndex;

        protected internal LoopIndexController()
        {
            oldLoopIndex = loopIndex;
        }

        public void Next(int newIndex) => loopIndex = newIndex;

        public void Dispose()
        {
            loopIndex = oldLoopIndex;
        }
    }

    public static LoopIndexController StartLoopIndex() => new();

    public static ValueRemembered<T> Get<T>(Func<T> creator)
    {
        var key = GetKey();
        var current = Composer.Instance.Current!;
        if (current.Remembered.TryGetNextRemembered<T>(key, out var result))
        {
            return result;
        }

        var value = creator();

        return current.Remembered.AddRemembered(key, value);
    }

    public static ValueRemembered<T> GetEnumerable<T, V>(Func<T> creator) where T : IEnumerable<V>
    {
        throw new NotImplementedException();
        var key = GetKey();
        var current = Composer.Instance.Current!;
        if (current.Remembered.TryGetNextRemembered<T>(key, out var result))
        {
            return result;
        }

        var value = creator();

        return current.Remembered.AddRemembered(key, value);
    }

    private static string GetKey()
    {
        var st = new StackTrace();
        var key = new StringBuilder();
        foreach (var stackFrame in st.GetFrames())
        {
            key.Append($"{stackFrame.GetILOffset()}-");

            if (stackFrame.GetMethod()?.GetCustomAttribute(typeof(RootComposableAttribute)) != null)
                break;
        }

        key.Append(loopIndex);

        return key.ToString();
    }

    public static ValueRemembered<T> Get<T>(T value)
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