namespace SharpCompose.Base;

public class LaunchedEffectAsyncImpl : IRememberObserver, ILaunchedEffect
{
    private readonly Func<CancellationToken, Task> task;
    private Action? disposeCallback;
    private CancellationTokenSource cancellationSource;

    public LaunchedEffectAsyncImpl(Func<CancellationToken, Task> task)
    {
        this.task = task;
        cancellationSource = new CancellationTokenSource();
    }

    public void OnRemember()
    {
        Task.Run(async () => await task(cancellationSource.Token));
    }

    public void OnForgotten()
    {
        cancellationSource.Cancel();
        disposeCallback?.Invoke();
        cancellationSource = new CancellationTokenSource();
    }

    public void OnDispose(Action onDisposeAction)
    {
        disposeCallback = onDisposeAction;
    }
}