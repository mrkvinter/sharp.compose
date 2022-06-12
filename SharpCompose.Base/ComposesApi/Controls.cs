﻿using SharpCompose.Base.ComposesApi.Providers;
using SharpCompose.Base.Modifiers;
using SharpCompose.Base.Modifiers.Extensions;
using SharpCompose.Drawer.Core.Brushes;

namespace SharpCompose.Base.ComposesApi;

public partial class BaseCompose
{
    private readonly record struct ButtonState(bool IsHovered, bool IsPressed);

    public static void Button(Action onClick, string label, Modifier? modifier = null)
    {
        var contentBorder =
            new LinearGradientBrush("#14FFFFFF".AsColor(), "#000000".AsColor(), (0.5f, 0.95f), (0.5f, 1));
        var buttonState = Remember.Get(new ButtonState());
        var alpha = buttonState.Value switch
        {
            {IsPressed: true} => 0.8f,
            {IsHovered: true} => 0.9f,
            {IsHovered: false, IsPressed: false} => 1
        };
        var buttonColor = LocalProviders.Colors.Value.Accent.WithAlpha(alpha);

        Box(Modifier.With
                .Clip(Shapes.RoundCorner(4))
                .BackgroundColor(buttonColor)
                .Border(contentBorder, 1, Shapes.RoundCorner(4))
                .OnMouseOver(() => buttonState.Value = buttonState.Value with {IsHovered = true})
                .OnMouseOut(() => buttonState.Value = buttonState.Value with {IsHovered = false})
                .OnMouseDown(() =>
                {
                    if (buttonState.Value.IsHovered) buttonState.Value = buttonState.Value with {IsPressed = true};
                })
                .OnMouseUp(() =>
                {
                    if (buttonState.Value.IsPressed && buttonState.Value.IsHovered)
                        onClick();

                    buttonState.Value = buttonState.Value with {IsPressed = false};
                })
                .Padding(24, 12)
                .Then(modifier ?? Modifier.With),
            content: () =>
            {
                var textColor = LocalProviders.Colors.Value.OnAccent;
                textColor = buttonState.Value.IsPressed ? textColor.WithAlpha(0.7f) : textColor;
                Text(text: label, color: textColor);
            });
    }
}