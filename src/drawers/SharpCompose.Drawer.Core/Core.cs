using System.Drawing;
using SharpCompose.Drawer.Core.Brushes;

namespace SharpCompose.Drawer.Core;

public struct Border
{
    public readonly Brush Brush;
    public readonly int Width;

    public Border()
    {
        Brush = SolidColorBrush.White;
        Width = 0;
    }
    
    public Border(Brush brush, int width)
    {
        Brush = brush;
        Width = width;
    }
}

public readonly record struct Corners(int TopLeft, int TopRight, int BottomLeft, int BottomRight)
{
    public bool HasAnyCorner => TopLeft + TopRight + BottomLeft + BottomRight > 0;
}

public readonly record struct Rect(int X, int Y, int Width, int Height);

public readonly record struct ShadowInfo(Color Color, (int x, int y) Offset, float BlurRadius);

public class Font
{
    public string FontFamily { get; }

    public FontWeight FontWeight { get; }
    
    public Stream? FontStream { get; init; }

    public Font(string fontFamily, FontWeight fontWeight)
    {
        FontFamily = fontFamily;
        FontWeight = fontWeight;
    }
}

public struct FontWeight
{
    public static readonly FontWeight Thin = new(100);
    public static readonly FontWeight ExtraLight = new(200);
    public static readonly FontWeight Light = new(300);
    public static readonly FontWeight Regular = new(400);
    public static readonly FontWeight Medium = new(500);
    public static readonly FontWeight SemiBold = new(600);
    public static readonly FontWeight Bold = new(700);
    public static readonly FontWeight ExtraBold = new(800);
    public static readonly FontWeight Black = new(900);

    public int Weight { get; } = 100;

    private FontWeight(int weight)
    {
        Weight = weight;
    }
}
