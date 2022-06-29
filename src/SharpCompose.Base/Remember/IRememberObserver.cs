namespace SharpCompose.Base;

public interface IRememberObserver
{
    void OnRemember();

    void OnForgotten();
}