using SharpCompose.Base.ComposesApi;
using SharpCompose.Base.Extensions;
using SharpCompose.Base.Input;

namespace SharpCompose.Base.Modifiers.Extensions;

public static class InputModifierExtensions
{
    public static T Clickable<T>(this T self, Action callback) where T : IScopeModifier<T>
    {
        var buttonState = Remember.Get(() => new BaseCompose.ButtonState().AsMutableState());

        return self
            .OnMouseOver(() => buttonState.Value = buttonState.Value with {IsHovered = true})
            .OnMouseOut(() => buttonState.Value = buttonState.Value with {IsHovered = false})
            .OnMouseDown(() =>
            {
                if (buttonState.Value.IsHovered) buttonState.Value = buttonState.Value with {IsPressed = true};
            })
            .OnMouseUp(() =>
            {
                if (buttonState.Value.IsPressed && buttonState.Value.IsHovered)
                    callback();

                buttonState.Value = buttonState.Value with {IsPressed = false};
            });
    }

    public static T OnMouseOver<T>(this T self, Action callback) where T : IScopeModifier<T>
    {
        var boundState = Remember.Get(() => new BoundState().AsMutableState());
        return OnInputModifier(self, new OnMouseInputModifier(boundState)
        {
            OnMouseOver = callback
        }, boundState);
    }

    public static T OnMouseOut<T>(this T self, Action callback) where T : IScopeModifier<T>
    {
        var boundState = Remember.Get(() => new BoundState().AsMutableState());
        return OnInputModifier(self, new OnMouseInputModifier(boundState)
        {
            OnMouseOut = callback
        }, boundState);
    }

    public static T OnMouseDown<T>(this T self, Action callback) where T : IScopeModifier<T>
    {
        var boundState = Remember.Get(() => new BoundState().AsMutableState());
        return OnInputModifier(self, new OnMouseInputModifier(boundState)
        {
            OnMouseDown = callback
        }, boundState);
    }

    public static T OnMouseUp<T>(this T self, Action callback) where T : IScopeModifier<T>
    {
        var boundState = Remember.Get(() => new BoundState().AsMutableState());
        return OnInputModifier(self, new OnMouseInputModifier(boundState)
        {
            OnMouseUp = callback
        }, boundState);
    }

    public static T OnMouseUp<T>(this T self, Action<BoundState> callback) where T : IScopeModifier<T>
    {
        var boundState = Remember.Get(() => new BoundState().AsMutableState());
        return OnInputModifier(self, new OnMouseInputModifier(boundState)
        {
            OnMouseUp = () => callback(boundState.Value)
        }, boundState);
    }

    private static T OnInputModifier<T>(
        T modifier,
        OnMouseInputModifier mouseInputModifier,
        MutableState<BoundState> boundMutableState) where T : IScopeModifier<T>
    {
        var inputHandler = BaseCompose.LocalInputHandler.Value;
        var mouseState = Remember.Get(
            () => new MouseState(IsMouseOver(inputHandler.MousePosition.x,
                inputHandler.MousePosition.y, boundMutableState.Value)).AsMutableState());

        void OnMouseMove(int mouseX, int mouseY)
        {
            if (IsMouseOver(mouseX, mouseY, boundMutableState.Value) && !mouseState.Value.IsOver)
            {
                mouseInputModifier.OnMouseOver?.Invoke();
                mouseState.Value = new MouseState(true);
            }

            if (!IsMouseOver(mouseX, mouseY, boundMutableState.Value) && mouseState.Value.IsOver)
            {
                mouseState.Value = new MouseState(false);
                mouseInputModifier.OnMouseOut?.Invoke();
            }
        }

        void OnMouseDownAction() => mouseInputModifier.OnMouseDown?.Invoke();
        void OnMouseUpAction() => mouseInputModifier.OnMouseUp?.Invoke();

        Remember
            .LaunchedEffect(true, () => inputHandler.MouseMove += OnMouseMove)
            .OnDispose(() => inputHandler.MouseMove -= OnMouseMove);

        Remember
            .LaunchedEffect(true, () => inputHandler.MouseDown += OnMouseDownAction)
            .OnDispose(() => inputHandler.MouseDown -= OnMouseDownAction);

        Remember
            .LaunchedEffect(true, () => inputHandler.MouseUp += OnMouseUpAction)
            .OnDispose(() => inputHandler.MouseUp -= OnMouseUpAction);

        return modifier.Then(mouseInputModifier);
    }

    private static bool IsMouseOver(int mouseX, int mouseY, BoundState boundState)
        => mouseX >= boundState.XOffset && mouseX <= boundState.XOffset + boundState.Width &&
           mouseY >= boundState.YOffset && mouseY <= boundState.YOffset + boundState.Height;

    public static T OnInputKeyDown<T>(
        this T modifier,
        Action<KeyCode> onInputKeyDown) where T : IScopeModifier<T>
    {
        var inputHandler = BaseCompose.LocalInputHandler.Value;

        Remember
            .LaunchedEffect(true, () => inputHandler.KeyDown += onInputKeyDown)
            .OnDispose(() => inputHandler.KeyDown -= onInputKeyDown);

        return modifier;
    }

    public static T OnInputText<T>(
        this T modifier,
        Action<string> onInputText) where T : IScopeModifier<T>
    {
        var inputHandler = BaseCompose.LocalInputHandler.Value;

        Remember.LaunchedEffect(true, () => inputHandler.OnTextInput += onInputText)
            .OnDispose(() => inputHandler.OnTextInput -= onInputText);

        return modifier;
    }
}

public record struct BoundState(int XOffset = 0, int YOffset = 0, int Width = 0, int Height = 0);