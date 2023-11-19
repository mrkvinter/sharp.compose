using SharpCompose.Base.Input;

namespace SharpCompose.Base;

public interface IInputHandler
{
    (int x, int y) MousePosition { get; }

    void SubscribeMouseDown(string uniqKey, Action callback);
    void DisposeMouseDown(string uniqKey);

    void SubscribeMouseUp(string uniqKey, Action callback);
    void DisposeMouseUp(string uniqKey);

    void SubscribeMouseMove(string uniqKey, Action<int, int> callback);
    void DisposeMouseMove(string uniqKey);

    void SubscribeKeyDown(string uniqKey, Action<KeyCode> callback);
    void DisposeKeyDown(string uniqKey);

    void SubscribeTextInput(string uniqKey, Action<string> callback);
    void DisposeTextInput(string uniqKey);

    void SetCursor(Cursor cursor);
}

public enum Cursor
{
    Default,
    Pointer,
    Text
}