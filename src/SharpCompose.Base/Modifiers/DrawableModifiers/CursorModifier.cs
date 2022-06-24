using System.Drawing;
using SharpCompose.Base.ComposesApi.Providers;
using SharpCompose.Base.Modifiers.Extensions;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;

namespace SharpCompose.Base.Modifiers.DrawableModifiers;

public static class CursorModifier
{
    public static T Cursor<T>(this T self, string text, int cursorPosition) where T : IScopeModifier<T>
    {
        var fontSize = LocalProviders.TextStyle.Value.FontSize; 
        var font = LocalProviders.TextStyle.Value.Font;
        var alpha = Remember.Get(cursorPosition, () => 0f);
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

            var (offset, _) = graphics.MeasureText(text[..cursorPosition], fontSize, font);
            graphics.FillRectangle(pos with {X = pos.X + offset - 1}, size with {Width = 1}, new SolidColorBrush(Color.Black.WithAlpha(alpha.Value)));
        }));
    }
    
    public static T OnTextClick<T>(this T self, string text, ValueRemembered<int> cursorPosition) where T : IScopeModifier<T>
    {
        var carriageSize = Remember.Get(Array.Empty<int>());
        var fontSize = LocalProviders.TextStyle.Value.FontSize; 
        var font = LocalProviders.TextStyle.Value.Font;
        var inputHandler = LocalProviders.InputHandler.Value!;

        
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
            var (w, _) = graphics.MeasureText(c.ToString(), fontSize, font);
            array[i] = w;
        }

        array[^1] = graphics.MeasureText("_", fontSize, font).Width;
        return array;
    }
}