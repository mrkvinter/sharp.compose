using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace SharpCompose.Base;

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

    [ComposableApi]
    private static void ForgetInternal<T>(string postfixKey)
    {
        var key = GetKey(postfixKey);
        var current = Composer.Instance.Current!;
        if (!current.Remembered.TryGetNextRemembered<T>(key, out var result)) return;
        if (result is IRememberObserver rememberObserver) rememberObserver.OnForgotten();
        current.Remembered.RemoveRemembered(key);
    }

    [ComposableApi]
    private static T GetInternal<T>(string postfixKey, Func<T> creator)
    {
        var key = GetKey(postfixKey);
        var current = Composer.Instance.Current!;
        if (current.Remembered.TryGetNextRemembered<T>(key, out var result))
        {
            return result;
        }

        var value = creator();

        current.Remembered.AddRemembered(key, value);
        if (value is IRememberObserver rememberObserver) rememberObserver.OnRemember();

        return value;
    }

    [ComposableApi]
    public static T Get<T>(Func<T> creator)
    {
        var key = GetKey();
        var current = Composer.Instance.Current!;
        if (current.Remembered.TryGetNextRemembered<T>(key, out var result))
        {
            return result;
        }

        var value = creator();

        current.Remembered.AddRemembered(key, value);
        if (value is IRememberObserver rememberObserver) rememberObserver.OnRemember();

        return value;
    }

    [ComposableApi]
    public static T Get<TKey, T>(TKey key, Func<T> creator)
    {
        const string keyPostfix = "_key";
        const string valuePostfix = "_val";
        var rememberedKey = GetInternal(keyPostfix, () => key);

        if (!rememberedKey!.Equals(key))
            ForgetInternal<T>(valuePostfix);

        return GetInternal(valuePostfix, creator);
    }

    [ComposableApi]
    public static T Get<TKey1, TKey2, T>(TKey1 key1, TKey2 key2, Func<T> creator)
    {
        const string key1Postfix = "_key1";
        const string key2Postfix = "_key2";
        const string valuePostfix = "_val";
        var rememberedKey1 = GetInternal(key1Postfix, () => key1);
        var rememberedKey2 = GetInternal(key2Postfix, () => key2);

        if (!rememberedKey1!.Equals(key1) || !rememberedKey2!.Equals(key2))
            ForgetInternal<T>(valuePostfix);

        return GetInternal(valuePostfix, creator);
    }

    [ComposableApi]
    public static T Get<TKey1, TKey2, TKey3, T>(TKey1 key1, TKey2 key2, TKey3 key3, Func<T> creator)
    {
        const string key1Postfix = "_key1";
        const string key2Postfix = "_key2";
        const string key3Postfix = "_key3";
        const string valuePostfix = "_val";
        var rememberedKey1 = GetInternal(key1Postfix, () => key1);
        var rememberedKey2 = GetInternal(key2Postfix, () => key2);
        var rememberedKey3 = GetInternal(key3Postfix, () => key3);

        if (!rememberedKey1!.Equals(key1) || !rememberedKey2!.Equals(key2) || !rememberedKey3!.Equals(key3))
            ForgetInternal<T>(valuePostfix);

        return GetInternal(valuePostfix, creator);
    }


    [ComposableApi]
    private static string GetKey(string postfix = "")
    {
        var st = new StackTrace();
        var key = new StringBuilder();
        foreach (var stackFrame in st.GetFrames())
        {
            if (stackFrame.GetMethod()?.GetCustomAttribute<ComposableApiAttribute>() is { } apiAttribute)
            {
                if (apiAttribute is RootComposableApiAttribute)
                    break;

                continue;
            }

            key.Append($"{stackFrame.GetNativeOffset()}-");
        }

        key.Append(LoopIndex);
        key.Append(postfix);

        return key.ToString();
    }

    [ComposableApi]
    public static ILaunchedEffect LaunchedEffect<T>(T key, Func<CancellationToken, Task> action)
        => Get(key, () => new LaunchedEffectImpl(action));

    [ComposableApi]
    public static ILaunchedEffect LaunchedEffect<T>(T key, Action action)
        => Get(key, () => new LaunchedEffectImpl(_ =>
        {
            action();
            return Task.CompletedTask;
        }));

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