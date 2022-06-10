namespace SharpCompose.Base;

public interface IInputHandler
{
    (int x, int y) MousePosition { get; }

    event Action MouseDown;

    event Action MouseUp;

    event Action<int, int> MouseMove;
}