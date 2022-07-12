namespace SharpCompose.Base;

public static class Remember
{
    [ComposableApi]
    private static void ForgetInternal<T>(string postfixKey)
    {
        var key = ComposeKey.GetKey(postfixKey);
        var current = Composer.Instance.CurrentGroup;
        if (!current.Remembered.TryGetNextRemembered<T>(key, out var result)) return;
        if (result is IRememberObserver rememberObserver) rememberObserver.OnForgotten();
        current.Remembered.RemoveRemembered(key);
    }

    [ComposableApi]
    internal static T GetInternal<T>(string postfixKey, Func<T> creator) 
        where T : notnull
    {
        var key = ComposeKey.GetKey(postfixKey);
        var current = Composer.Instance.CurrentGroup;
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
        where T : notnull
    {
        var key = ComposeKey.GetKey();
        var current = Composer.Instance.CurrentGroup;
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
        where TKey : notnull 
        where T : notnull
    {
        const string keyPostfix = "_key";
        const string valuePostfix = "_val";
        var rememberedKey = GetInternal(keyPostfix, () => key);

        if (!rememberedKey.Equals(key))
        {
            ForgetInternal<TKey>(keyPostfix);
            GetInternal(keyPostfix, () => key);

            ForgetInternal<T>(valuePostfix);
        }

        return GetInternal(valuePostfix, creator);
    }

    [ComposableApi]
    public static T Get<TKey1, TKey2, T>(TKey1 key1, TKey2 key2, Func<T> creator)
        where TKey1 : notnull 
        where TKey2 : notnull 
        where T : notnull
    {
        const string key1Postfix = "_key1";
        const string key2Postfix = "_key2";
        const string valuePostfix = "_val";
        var rememberedKey1 = GetInternal(key1Postfix, () => key1);
        var rememberedKey2 = GetInternal(key2Postfix, () => key2);

        if (!rememberedKey1.Equals(key1) || !rememberedKey2.Equals(key2))
        {
            ForgetInternal<TKey1>(key1Postfix);
            GetInternal(key1Postfix, () => key1);

            ForgetInternal<TKey2>(key2Postfix);
            GetInternal(key2Postfix, () => key2);

            ForgetInternal<T>(valuePostfix);
        }

        return GetInternal(valuePostfix, creator);
    }

    [ComposableApi]
    public static T Get<TKey1, TKey2, TKey3, T>(TKey1 key1, TKey2 key2, TKey3 key3, Func<T> creator)
        where TKey1 : notnull 
        where TKey2 : notnull 
        where TKey3 : notnull 
        where T : notnull
    {
        const string key1Postfix = "_key1";
        const string key2Postfix = "_key2";
        const string key3Postfix = "_key3";
        const string valuePostfix = "_val";
        var rememberedKey1 = GetInternal(key1Postfix, () => key1);
        var rememberedKey2 = GetInternal(key2Postfix, () => key2);
        var rememberedKey3 = GetInternal(key3Postfix, () => key3);

        if (!rememberedKey1.Equals(key1) || !rememberedKey2.Equals(key2) || !rememberedKey3.Equals(key3))
        {
            ForgetInternal<TKey1>(key1Postfix);
            GetInternal(key1Postfix, () => key1);

            ForgetInternal<TKey2>(key2Postfix);
            GetInternal(key2Postfix, () => key2);

            ForgetInternal<TKey3>(key3Postfix);
            GetInternal(key3Postfix, () => key3);

            ForgetInternal<T>(valuePostfix);
        }

        return GetInternal(valuePostfix, creator);
    }

    [ComposableApi]
    public static T Get<TKey1, TKey2, TKey3, TKey4, T>(TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, Func<T> creator)
        where TKey1 : notnull
        where TKey2 : notnull
        where TKey3 : notnull
        where TKey4 : notnull
        where T : notnull
    {
        const string key1Postfix = "_key1";
        const string key2Postfix = "_key2";
        const string key3Postfix = "_key3";
        const string key4Postfix = "_key4";
        const string valuePostfix = "_val";
        var rememberedKey1 = GetInternal(key1Postfix, () => key1);
        var rememberedKey2 = GetInternal(key2Postfix, () => key2);
        var rememberedKey3 = GetInternal(key3Postfix, () => key3);
        var rememberedKey4 = GetInternal(key4Postfix, () => key4);

        if (!rememberedKey1.Equals(key1) || !rememberedKey2.Equals(key2) || !rememberedKey3.Equals(key3) ||
            !rememberedKey4.Equals(key4))
        {
            ForgetInternal<TKey1>(key1Postfix);
            GetInternal(key1Postfix, () => key1);

            ForgetInternal<TKey2>(key2Postfix);
            GetInternal(key2Postfix, () => key2);

            ForgetInternal<TKey3>(key3Postfix);
            GetInternal(key3Postfix, () => key3);

            ForgetInternal<TKey4>(key4Postfix);
            GetInternal(key4Postfix, () => key4);

            ForgetInternal<T>(valuePostfix);
        }

        return GetInternal(valuePostfix, creator);
    }

    [ComposableApi]
    public static ILaunchedEffect LaunchedEffect<TKey>(TKey key, Func<CancellationToken, Task> action) 
        where TKey : notnull 
        => Get(key, () => new LaunchedEffectImpl(action));

    [ComposableApi]
    public static ILaunchedEffect LaunchedEffect<TKey>(TKey key, Action action)
        where TKey : notnull
        => Get(key, () => new LaunchedEffectImpl(_ =>
        {
            action();
            return Task.CompletedTask;
        }));
}