using System.Drawing;

namespace SharpCompose.Drawer.Core.Brushes;

public sealed class SolidColorBrush : Brush
{
    public static readonly SolidColorBrush White = new(Color.White);
    public static readonly SolidColorBrush Transparent = new(Color.Transparent);
    public Color Color { get; }

    public SolidColorBrush(Color color)
    {
        Color = color;
    }
}