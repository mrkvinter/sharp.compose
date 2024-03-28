using SharpCompose.Base.Input;

namespace SharpCompose.Base;

public class InputHandler : IInputHandler
{
    private readonly Dictionary<string, Action> mouseDown = new();
    private readonly Dictionary<string, Action> mouseUp = new();
    private readonly Dictionary<string, Action<int, int>> mouseMove = new();
    private readonly Dictionary<string, Action<KeyCode>> keyDown = new();
    private readonly Dictionary<string, Action<string>> textInput = new();
    private readonly Action<Cursor> changeCursor;

    public (int x, int y) MousePosition { get; private set; }
    
    public InputHandler(Action<Cursor> changeCursor)
    {
        this.changeCursor = changeCursor;
        MousePosition = (-1, -1);
    }

    public void OnMouseDown()
    {
        foreach (var (_, callback) in mouseDown)
        {
            callback();
        }
    }
    
    public void OnMouseUp()
    {
        foreach (var (_, callback) in mouseUp)
        {
            callback();
        }
    }
    
    public void OnMouseMove(int x, int y)
    {
        MousePosition = (x, y);
        foreach (var (_, callback) in mouseMove)
        {
            callback(x, y);
        }
    }

    public void OnKeyDown(KeyCode keyCode)
    {
        foreach (var (_, callback) in keyDown)
        {
            callback(keyCode);
        }
    }
    
    public void SubscribeMouseDown(string uniqKey, Action callback) => mouseDown.Add(uniqKey, callback);
    public void DisposeMouseDown(string uniqKey) => mouseDown.Remove(uniqKey);


    public void SubscribeMouseUp(string uniqKey, Action callback) => mouseUp.Add(uniqKey,callback);
    public void DisposeMouseUp(string uniqKey) => mouseUp.Remove(uniqKey);

    public void SubscribeMouseMove(string uniqKey, Action<int, int> callback) => mouseMove.Add(uniqKey,callback);
    public void DisposeMouseMove(string uniqKey) => mouseMove.Remove(uniqKey);

    public void SubscribeKeyDown(string uniqKey, Action<KeyCode> callback) => keyDown.Add(uniqKey,callback);
    public void DisposeKeyDown(string uniqKey) => keyDown.Remove(uniqKey);

    public void SubscribeTextInput(string uniqKey, Action<string> callback) => textInput.Add(uniqKey,callback);
    public void DisposeTextInput(string uniqKey) => textInput.Remove(uniqKey);
    public void SetCursor(Cursor cursor) => changeCursor.Invoke(cursor);
    public bool Equals(IInputHandler? other)
    {
        return other is InputHandler;
    }
}