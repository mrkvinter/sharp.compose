using System;

namespace SharpCompose.WinUI;

public static class ComposerHotReloader
{
    public static Action HotReload;

    public static void UpdateApplication(Type[]? _)
    {
        HotReload?.Invoke();
    }
}