using SharpCompose.Base.ComposesApi.Providers;

namespace SharpCompose.Base.Modifiers.Extensions;

public static class InputModifierExtensions
{
    public static T OnMouseOver<T>(this T self, Action callback) where T : IScopeModifier<T>
    {
        var boundState = Remember.Get(new BoundState());
        return OnInputModifier(self, new OnMouseInputModifier(boundState)
        {
            OnMouseOver = callback
        }, boundState);
    }

    public static T OnMouseOut<T>(this T self, Action callback) where T : IScopeModifier<T>
    {
        var boundState = Remember.Get(new BoundState());
        return OnInputModifier(self, new OnMouseInputModifier(boundState)
        {
            OnMouseOut = callback
        }, boundState);
    }

    public static T OnMouseDown<T>(this T self, Action callback) where T : IScopeModifier<T>
    {
        var boundState = Remember.Get(new BoundState());
        return OnInputModifier(self, new OnMouseInputModifier(boundState)
        {
            OnMouseDown = callback
        }, boundState);
    }

    public static T OnMouseUp<T>(this T self, Action callback) where T : IScopeModifier<T>
    {
        var boundState = Remember.Get(new BoundState());
        return OnInputModifier(self, new OnMouseInputModifier(boundState)
        {
            OnMouseUp = callback
        }, boundState);
    }

    private static T OnInputModifier<T>(
        T modifier, 
        OnMouseInputModifier mouseInputModifier,
        ValueRemembered<BoundState> boundState) where T : IScopeModifier<T>
    {
        var inputHandler = LocalProviders.InputHandler.Value!;
        var mouseState = Remember.Get(
            () => new MouseState(IsMouseOver(inputHandler.MousePosition.x, inputHandler.MousePosition.y, boundState.Value)));

        void OnMouseMove(int mouseX, int mouseY)
        {
            if (IsMouseOver(mouseX, mouseY, boundState.Value) && !mouseState.Value.IsOver)
            {
                mouseInputModifier.OnMouseOver?.Invoke();
                mouseState.Value = new MouseState(true);
            }

            if (!IsMouseOver(mouseX, mouseY, boundState.Value) && mouseState.Value.IsOver)
            {
                mouseState.Value = new MouseState(false);
                mouseInputModifier.OnMouseOut?.Invoke();
            }
        }

        void OnMouseDownAction() => mouseInputModifier.OnMouseDown?.Invoke();
        void OnMouseUpAction() => mouseInputModifier.OnMouseUp?.Invoke();
        
        Remember.LaunchedEffect(() => inputHandler.MouseMove += OnMouseMove);
        Remember.LaunchedEffect(() => inputHandler.MouseDown += OnMouseDownAction);
        Remember.LaunchedEffect(() => inputHandler.MouseUp += OnMouseUpAction);

        Remember.DisposeEffect(() => inputHandler.MouseMove -= OnMouseMove);
        Remember.DisposeEffect(() => inputHandler.MouseDown -= OnMouseDownAction);
        Remember.DisposeEffect(() => inputHandler.MouseUp -= OnMouseUpAction);
        
        return modifier.Then(mouseInputModifier);
    }

    private static bool IsMouseOver(int mouseX, int mouseY, BoundState boundState)
        => mouseX >= boundState.XOffset && mouseX <= boundState.XOffset + boundState.Width &&
           mouseY >= boundState.YOffset && mouseY <= boundState.YOffset + boundState.Height;
}

public record struct BoundState(int XOffset = 0, int YOffset = 0, int Width = 0, int Height = 0);
