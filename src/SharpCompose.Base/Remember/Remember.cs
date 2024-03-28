namespace SharpCompose.Base;

public static class Remember
{
    [ComposableApi]
    private static void ForgetInternal<T>(string key)
    {
        var current = Composer.Instance.CurrentGroup;
        if (!current.Remembered.TryGetNextRemembered<T>(key, out var result)) return;
        if (result is IRememberObserver rememberObserver) rememberObserver.OnForgotten();
        current.Remembered.RemoveRemembered(key);
    }

    [ComposableApi]
    private static T GetInternal<T>(string key, Func<T> creator) 
        where T : notnull
    {
        var current = Composer.Instance.CurrentGroup;
        current.UnusedRememberedKeys.Remove(key);
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
        current.UnusedRememberedKeys.Remove(key);
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
        var keyKey = ComposeKey.GetKey("_key");
        var valueKey = ComposeKey.GetKey("_val");
        var rememberedKey = GetInternal(keyKey, () => key);

        if (!rememberedKey.Equals(key))
        {
            ForgetInternal<TKey>(keyKey);
            GetInternal(keyKey, () => key);

            ForgetInternal<T>(valueKey);
        }

        return GetInternal(valueKey, creator);
    }

    [ComposableApi]
    public static T Get<TKey1, TKey2, T>(TKey1 key1, TKey2 key2, Func<T> creator)
        where TKey1 : notnull 
        where TKey2 : notnull 
        where T : notnull
    {
        var keyKey1 = ComposeKey.GetKey("_key1");
        var keyKey2 = ComposeKey.GetKey("_key2");
        var valueKey = ComposeKey.GetKey("_val");
        var rememberedKey1 = GetInternal(keyKey1, () => key1);
        var rememberedKey2 = GetInternal(keyKey2, () => key2);

        if (!rememberedKey1.Equals(key1) || !rememberedKey2.Equals(key2))
        {
            ForgetInternal<TKey1>(keyKey1);
            GetInternal(keyKey1, () => key1);

            ForgetInternal<TKey2>(keyKey2);
            GetInternal(keyKey2, () => key2);

            ForgetInternal<T>(valueKey);
        }

        return GetInternal(valueKey, creator);
    }

    [ComposableApi]
    public static T Get<TKey1, TKey2, TKey3, T>(TKey1 key1, TKey2 key2, TKey3 key3, Func<T> creator)
        where TKey1 : notnull 
        where TKey2 : notnull 
        where TKey3 : notnull 
        where T : notnull
    {
        var keyKey1 = ComposeKey.GetKey("_key1");
        var keyKey2 = ComposeKey.GetKey("_key2");
        var keyKey3 = ComposeKey.GetKey("_key3");
        var valueKey = ComposeKey.GetKey("_val");
        var rememberedKey1 = GetInternal(keyKey1, () => key1);
        var rememberedKey2 = GetInternal(keyKey2, () => key2);
        var rememberedKey3 = GetInternal(keyKey3, () => key3);

        if (!rememberedKey1.Equals(key1) || !rememberedKey2.Equals(key2) || !rememberedKey3.Equals(key3))
        {
            ForgetInternal<TKey1>(keyKey1);
            GetInternal(keyKey1, () => key1);

            ForgetInternal<TKey2>(keyKey2);
            GetInternal(keyKey2, () => key2);

            ForgetInternal<TKey3>(keyKey3);
            GetInternal(keyKey3, () => key3);

            ForgetInternal<T>(valueKey);
        }

        return GetInternal(valueKey, creator);
    }

    [ComposableApi]
    public static T Get<TKey1, TKey2, TKey3, TKey4, T>(TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, Func<T> creator)
        where TKey1 : notnull
        where TKey2 : notnull
        where TKey3 : notnull
        where TKey4 : notnull
        where T : notnull
    {
        var keyKey1 = ComposeKey.GetKey("_key1");
        var keyKey2 = ComposeKey.GetKey("_key2");
        var keyKey3 = ComposeKey.GetKey("_key3");
        var keyKey4 = ComposeKey.GetKey("_key4");
        var valueKey = ComposeKey.GetKey("_val");
        var rememberedKey1 = GetInternal(keyKey1, () => key1);
        var rememberedKey2 = GetInternal(keyKey2, () => key2);
        var rememberedKey3 = GetInternal(keyKey3, () => key3);
        var rememberedKey4 = GetInternal(keyKey4, () => key4);

        if (!rememberedKey1.Equals(key1) || !rememberedKey2.Equals(key2) || !rememberedKey3.Equals(key3) ||
            !rememberedKey4.Equals(key4))
        {
            ForgetInternal<TKey1>(keyKey1);
            GetInternal(keyKey1, () => key1);

            ForgetInternal<TKey2>(keyKey2);
            GetInternal(keyKey2, () => key2);

            ForgetInternal<TKey3>(keyKey3);
            GetInternal(keyKey3, () => key3);

            ForgetInternal<TKey4>(keyKey4);
            GetInternal(keyKey4, () => key4);

            ForgetInternal<T>(valueKey);
        }

        return GetInternal(valueKey, creator);
    }

    [ComposableApi]
    public static ILaunchedEffect LaunchedEffect<TKey>(TKey key, Func<CancellationToken, Task> action)
        where TKey : notnull
    {
        var launchedEffect = Get(key, () => new LaunchedEffectAsyncImpl(action));
        
        return launchedEffect;
    }

    [ComposableApi]
    public static ILaunchedEffect LaunchedEffect<TKey>(TKey key, Action action)
        where TKey : notnull
    {
        var launchedEffect = Get(key, () => new LaunchedEffectImpl(action));
        
        return launchedEffect;
    }
}