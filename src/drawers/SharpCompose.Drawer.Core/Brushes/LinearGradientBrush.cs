using System.Drawing;

namespace SharpCompose.Drawer.Core.Brushes;

public sealed class LinearGradientBrush : Brush
{
    public Color FirstColor { get; }
    public Color SecondColor { get; }
    public (float, float) FirstPoint { get; }
    public (float, float) SecondPoint { get; }

    public LinearGradientBrush(Color firstColor, Color secondColor, (float, float) firstPoint, (float, float) secondPoint)
    {
        FirstColor = firstColor;
        SecondColor = secondColor;
        FirstPoint = firstPoint;
        SecondPoint = secondPoint;
    }
}