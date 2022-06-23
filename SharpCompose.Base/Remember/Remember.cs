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
    private static readonly Stack<LoopIndexController> LoopIndexControllers = new();
    private static string LoopIndex => string.Join('-', LoopIndexControllers);

    public sealed class LoopIndexController : IDisposable
    {
        private int index;

        internal LoopIndexController()
        {
            LoopIndexControllers.Push(this);
        }

        public void Next(int newIndex) => index = newIndex;

        public void Dispose()
        {
            LoopIndexControllers.Pop();
        }

        public override string ToString() => index.ToString();
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

    public static ValueRemembered<T> Get<TKey1, T>(TKey1 key1, Func<T> creator, Action? onRemembered = null,
        Action? onForgotten = null)
    {
        var iKey1 = GetKey() + "key_1";
        var iKeyVal = GetKey() + "val";
        var current = Composer.Instance.Current!;
        var changed = false;
        var remembered = current.Remembered.TryGetNextRemembered<T>(iKeyVal, out var result);

        if (!current.Remembered.TryGetNextRemembered<TKey1>(iKey1, out var key1Val))
        {
            changed = true;
            current.Remembered.AddRemembered(iKey1, key1);
        }
        else if (!key1.Equals(key1Val.Value))
        {
            changed = true;
            key1Val.Value = key1;
        }

        if (remembered && changed)
            onForgotten?.Invoke();

        if (!remembered || changed)
        {
            onRemembered?.Invoke();

            var value = creator();

            if (!remembered)
                return current.Remembered.AddRemembered(iKeyVal, value);

            result.Value = value;
        }

        return result;
    }

    public static ValueRemembered<T> Get<T>(T value)
    {
        return Get(() => value);
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

        key.Append(LoopIndex);

        return key.ToString();
    }

    public static void LaunchedEffect(Action action)
    {
        Get<Unit>(() =>
        {
            action();

            return default;
        });
    }

    public static void LaunchedEffect(Func<Task> action)
    {
        Get<Unit>(() =>
        {
            action();

            return default;
        });
    }

    public static void LaunchedEffect<T>(T key, Func<CancellationToken, Task> action)
    {
        var cts = Get(new CancellationTokenSource());
        Get(key, 
            async () => { await action(cts.Value.Token); }, 
            () => cts.Value = new CancellationTokenSource(), 
            () => cts.Value.Cancel());
    }

    public static void DisposeEffect(Action action) => Get(() => new DisposableEffect(action));

    internal sealed class DisposableEffect : IDisposable
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