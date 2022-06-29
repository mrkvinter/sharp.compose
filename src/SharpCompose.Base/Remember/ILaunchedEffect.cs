namespace SharpCompose.Base;

public interface ILaunchedEffect
{
    void OnDispose(Action onDisposeAction);
}