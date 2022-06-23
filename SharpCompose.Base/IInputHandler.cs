using SharpCompose.Base.Input;

namespace SharpCompose.Base;

public interface IInputHandler
{
    (int x, int y) MousePosition { get; }

    event Action MouseDown;

    event Action MouseUp;

    event Action<int, int> MouseMove;
    
    event Action<KeyCode> KeyDown;

    event Action<string> OnTextInput; 


    void SetCursor(Cursor cursor);
}

public enum Cursor
{
    Default,
    Pointer,
    Text
}