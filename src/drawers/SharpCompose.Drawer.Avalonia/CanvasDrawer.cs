using System.Drawing;
using Avalonia.Controls;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;
using VectSharp;
using VectSharp.Canvas;
using Brush = SharpCompose.Drawer.Core.Brushes.Brush;
using Font = SharpCompose.Drawer.Core.Font;
using LinearGradientBrush = SharpCompose.Drawer.Core.Brushes.LinearGradientBrush;

namespace SharpCompose.Drawer.Avalonia;

public sealed class CanvasDrawer : ICanvas
{
    private readonly Action<Canvas> callback;
    private Page page;

    public CanvasDrawer(Action<Canvas> callback)
    {
        this.callback = callback;
    }

    public (int w, int h) Size { get; set; }

    public void Draw() => callback.Invoke(page.PaintToCanvas());

    public (int w, int h) MeasureText(string text, double emSize, Font font)
    {
        var measuredText = page.Graphics.MeasureText(text, new VectSharp.Font(font.Resolve(), emSize));
        return ((int) measuredText.Width, (int) measuredText.Height);
    }

    public IGraphics StartGraphics() => new AvaloniaGraphicsWrapper(new Graphics());

    public void DrawGraphics(int x, int y, IGraphics graphics)
    {
        page.Graphics.DrawGraphics(x, y, ((AvaloniaGraphicsWrapper) graphics).Graphics);
    }

    public void Clear()
    {
        page = new Page(Size.w, Size.h)
        {
            Background = Colours.White
        };
    }
}

public static class UtilsExtensions
{
    public static VectSharp.Brush ToVectBrush(this Brush brush, Rect rect) => brush switch
    {
        SolidColorBrush solidColorBrush => new SolidColourBrush(solidColorBrush.Color.ToColour()),

        LinearGradientBrush linearGradientBrush => linearGradientBrush.ToLinearGradientBrush(rect),

        _ => throw new ArgumentOutOfRangeException(nameof(brush))
    };

    public static Colour ToColour(this Color color) => Colour.FromRgba(color.R, color.G, color.B, color.A);

    private static VectSharp.LinearGradientBrush ToLinearGradientBrush(this LinearGradientBrush linearGradientBrush,
        Rect rect)
    {
        var firstPoint = new VectSharp.Point(rect.X + rect.Width * linearGradientBrush.FirstPoint.Item1,
            rect.Y + rect.Height * linearGradientBrush.FirstPoint.Item2);
        var secondPoint = new VectSharp.Point(rect.X + rect.Width * linearGradientBrush.SecondPoint.Item1,
            rect.Y + rect.Height * linearGradientBrush.SecondPoint.Item2);

        return new VectSharp.LinearGradientBrush(firstPoint, secondPoint,
            new GradientStop(linearGradientBrush.FirstColor.ToColour(), 0),
            new GradientStop(linearGradientBrush.SecondColor.ToColour(), 1));
    }
}