using System.Drawing;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;

namespace SharpCompose.Base.ElementBuilder;

public class TextElementBuilder : IElementBuilder
{
    public string? Text { get; init; }
    
    public int Size { get; init; }
    
    public Color Color { get; init; }

    public (int w, int h) CalculateVisualSize(Composer.Scope scope, ICanvas canvas)
    {
        var font = new Font("Courier", FontWeight.Regular);
        return canvas.MeasureText(Text ?? String.Empty, Size, font);
    }

    public (int w, int h) CalculateRealSize(Composer.Scope scope, ICanvas canvas)
        => CalculateVisualSize(scope, canvas);

    public void Draw(Composer.Scope scope, ICanvas canvas, int pointerX, int pointerY)
    {
        var font = new Font("Courier", FontWeight.Regular);

        canvas.DrawText(Text ?? String.Empty, Size, font,
            new SolidColorBrush(Color),
            new Point(pointerX, pointerY));
    }
}