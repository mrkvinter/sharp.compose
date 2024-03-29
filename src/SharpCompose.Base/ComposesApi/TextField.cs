﻿using System.Drawing;
using SharpCompose.Base.Extensions;
using SharpCompose.Base.Input;
using SharpCompose.Base.Modifiers.DrawableModifiers;
using SharpCompose.Base.Modifiers.Extensions;
using SharpCompose.Drawer.Core.Brushes;

namespace SharpCompose.Base.ComposesApi;

public partial class BaseCompose
{
    public static void TextField(string value, Action<string> onValueChanged)
    {
        var internalValue = Remember.Get(() => value.AsMutableState());
        internalValue.Value = value;

        var cursorPosition = Remember.Get(() => 0.AsMutableState());
        if (cursorPosition.Value > internalValue.Value.Length)
            cursorPosition.Value = internalValue.Value.Length;

        var borderBrush = new LinearGradientBrush(Color.Black.WithAlpha(0.06f), Color.Black.WithAlpha(0.44f),
            (0.5f, 0.99f), (0.5f, 1f));

        var inputHandler = LocalInputHandler.Value;
        Box(Modifier
                .Clip(Shapes.RoundCorner(4))
                .BackgroundColor(Color.White.WithAlpha(0.7f))
                .Border(borderBrush, 1, Shapes.RoundCorner(4))
                .MinSize(100, 0)
                .Padding(11, 4, 35, 6)
                .OnInputKeyDown(c =>
                {
                    switch (c)
                    {
                        case KeyCode.Left:
                            cursorPosition.Value -= cursorPosition.Value == 0 ? 0 : 1;
                            break;
                        case KeyCode.Right:
                            cursorPosition.Value += cursorPosition.Value == internalValue.Value.Length ? 0 : 1;
                            break;
                        case KeyCode.Back:
                        {
                            if (cursorPosition.Value != 0)
                            {
                                cursorPosition.Value--;
                                onValueChanged(internalValue.Value.Remove(cursorPosition.Value, 1));
                            }

                            break;
                        }
                        case KeyCode.Delete when cursorPosition.Value != internalValue.Value.Length:
                            onValueChanged(internalValue.Value.Remove(cursorPosition.Value, 1));
                            break;
                    }
                })
                .OnInputText(s =>
                {
                    onValueChanged(internalValue.Value.Insert(cursorPosition.Value, s));
                    cursorPosition.Value++;
                })
                .OnMouseOver(() => inputHandler.SetCursor(Cursor.Text))
            , content: () =>
            {
                Box(Modifier
                        .Cursor(internalValue.Value, cursorPosition.Value)
                        .OnTextClick(internalValue.Value, cursorPosition),
                    content: () => Text(internalValue.Value));
            });
    }


}