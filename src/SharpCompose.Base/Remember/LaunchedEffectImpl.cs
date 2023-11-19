namespace SharpCompose.Base;

public class LaunchedEffectImpl : IRememberObserver, ILaunchedEffect
{
    private readonly Action action;
    private Action? disposeCallback;

    public LaunchedEffectImpl(Action action)
    {
        this.action = action;
    }

    public void OnRemember()
    {
        action();
    }

    public void OnForgotten()
    {
        disposeCallback?.Invoke();
    }

    public void OnDispose(Action onDisposeAction)
    {
        disposeCallback = onDisposeAction;
    }
}