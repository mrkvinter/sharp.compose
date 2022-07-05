using System.Drawing;
using SharpCompose.Base.ComposesApi;
using SharpCompose.Base.Extensions;
using SharpCompose.Base.Modifiers.Extensions;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Utilities;

namespace SharpCompose.Base.Modifiers.DrawableModifiers;

public static class CursorModifier
{
    public static T Cursor<T>(this T self, string text, int cursorPosition) where T : IScopeModifier<T>
    {
        var fontSize = BaseCompose.LocalTextStyle.Value.FontSize; 
        var font = BaseCompose.LocalTextStyle.Value.Font;
        var alpha = Remember.Get(cursorPosition, () => 0f.AsMutableState());
        Remember.LaunchedEffect(cursorPosition, async ct =>
        {
            while (!ct.IsCancellationRequested)
            {
                alpha.Value = alpha.Value == 0 ? 1 : 0;
                await Task.Delay(500, ct);
            }
        });
        return self.Then(new DrawableModifier((graphics, size, pos) =>
        {

            var (offset, _) = graphics.MeasureText(text[..cursorPosition], fontSize, font, TextAlignment.Left, new IntSize(Int32.MaxValue, Int32.MaxValue));
            graphics.FillRectangle(pos with {X = pos.X + offset - 1}, size with {Width = 1}, new SolidColorBrush(Color.Black.WithAlpha(alpha.Value)));
        }));
    }
    
    public static T OnTextClick<T>(this T self, string text, MutableState<int> cursorPosition) where T : IScopeModifier<T>
    {
        var carriageSize = Remember.Get(() => Array.Empty<int>().AsMutableState());
        var fontSize = BaseCompose.LocalTextStyle.Value.FontSize; 
        var font = BaseCompose.LocalTextStyle.Value.Font;
        var inputHandler = BaseCompose.LocalInputHandler.Value;

        
        return self
            .Then(new DrawableModifier((graphics, _, _) => carriageSize.Value = CalculateSizeOfSelectableCarriageSpace(graphics, text, fontSize, font)))
            .OnMouseUp(b =>
            {
                var left = b.XOffset - (carriageSize.Value.Length > 0 ? carriageSize.Value[0] : 0) / 2f;
                for (var i = 0; i < carriageSize.Value.Length; i++)
                {
                    var right = left + carriageSize.Value[i];
                    if (inputHandler.MousePosition.x >= left && inputHandler.MousePosition.x < right)
                    {
                        cursorPosition.Value = i;
                        return;
                    }

                    left += carriageSize.Value[i];
                }
            });
    }

    private static int[] CalculateSizeOfSelectableCarriageSpace(IGraphics graphics, string text, double fontSize, Font font)
    {
        var array = new int[text.Length + 1];
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            var (w, _) = graphics.MeasureText(c.ToString(), fontSize, font, TextAlignment.Left, new IntSize(Int32.MaxValue, Int32.MaxValue));
            array[i] = w;
        }

        array[^1] = graphics.MeasureText("_", fontSize, font, TextAlignment.Left, new IntSize(Int32.MaxValue, Int32.MaxValue)).Width;
        return array;
    }
}