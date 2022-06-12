using System.Drawing;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Shapes;

namespace SharpCompose.Base;

public static class Shapes
{
    public static IShape Rectangle => new RectangleShape();

    public static IShape Circle => new CircleShape();

    public static IShape RoundCorner(float percent)
        => new RoundedCornerShape(
            new PercentCornerSize(percent),
            new PercentCornerSize(percent),
            new PercentCornerSize(percent),
            new PercentCornerSize(percent));

    public static IShape RoundCorner(int pixel)
        => new RoundedCornerShape(
            new PixelCornerSize(pixel),
            new PixelCornerSize(pixel),
            new PixelCornerSize(pixel),
            new PixelCornerSize(pixel));
}

public sealed class RectangleShape : IShape
{
    public Outline CreateOutline(int width, int height)
        => Outline.StartShape(0, 0).LineTo(width, 0).LineTo(width, height).LineTo(0, height).LineTo(0, 0).Close();
}

public interface ICornerSize
{
    int ToPx(int width, int height);
}

public class PercentCornerSize : ICornerSize
{
    private readonly float percent;

    public PercentCornerSize(float percent)
    {
        if (percent is not (>= 0 and <= 100))
            throw new ArgumentOutOfRangeException(nameof(percent));

        this.percent = percent;
    }

    public int ToPx(int width, int height) => (int) (Math.Min(width, height) * percent / 100.0f);
}

public class PixelCornerSize : ICornerSize
{
    private readonly int pixel;

    public PixelCornerSize(int pixel)
    {
        this.pixel = pixel;
    }

    public int ToPx(int width, int height) => pixel;
}

public sealed class RoundedCornerShape : IShape
{
    private readonly ICornerSize topLeft;
    private readonly ICornerSize topRight;
    private readonly ICornerSize bottomLeft;
    private readonly ICornerSize bottomRight;

    public RoundedCornerShape(
        ICornerSize topLeft,
        ICornerSize topRight,
        ICornerSize bottomLeft,
        ICornerSize bottomRight)
    {
        this.topLeft = topLeft;
        this.topRight = topRight;
        this.bottomLeft = bottomLeft;
        this.bottomRight = bottomRight;
    }

    public Outline CreateOutline(int width, int height)
    {
        const double circleCornerModifier = 0.44771525016;
        var topLeftShift = topLeft.ToPx(width, height);
        var topRightShift = topRight.ToPx(width, height);
        var bottomLeftShift = bottomLeft.ToPx(width, height);
        var bottomRightShift = bottomRight.ToPx(width, height);

        return Outline.StartShape(0, topLeftShift)
            .CubicBezierTo(
                topLeftShift, 0,
                0, (int) Math.Round(topLeftShift * circleCornerModifier),
                (int) Math.Round(topLeftShift * circleCornerModifier), 0)
            .LineTo(width - topRightShift, 0)
            .CubicBezierTo(
                width, topRightShift,
                width - (int) Math.Round(topRightShift * circleCornerModifier), 0,
                width, (int) Math.Round(topRightShift * (circleCornerModifier)))
            .LineTo(width, height - bottomRightShift)
            .CubicBezierTo(
                width - bottomRightShift, height,
                width, height - (int) Math.Round(bottomRightShift * circleCornerModifier),
                width - (int) Math.Round(bottomRightShift * circleCornerModifier), height)
            .LineTo(bottomLeftShift, height)
            .CubicBezierTo(
                0, height - bottomLeftShift,
                (int) Math.Round(bottomLeftShift * circleCornerModifier), height,
                0, height - (int) Math.Round(bottomLeftShift * circleCornerModifier))
            .LineTo(0, topLeftShift)
            .Close();
    }
}

public sealed class CircleShape : IShape
{
    public Outline CreateOutline(int width, int height)
    {
        const double circleCornerModifier = 0.44771525016;
        var halfWidth = (int) Math.Round(width / 2.0);
        var halfHeight = (int) Math.Round(height / 2.0);
        return Outline.StartShape(0, halfHeight)
            .CubicBezierTo(
                halfWidth, 0,
                0, (int) Math.Round(halfHeight * circleCornerModifier),
                (int) Math.Round(halfWidth * circleCornerModifier), 0)
            .CubicBezierTo(
                width, halfHeight,
                width - (int) Math.Round(halfWidth * circleCornerModifier), 0,
                width, (int) Math.Round(halfHeight * circleCornerModifier))
            .CubicBezierTo(
                halfWidth, height,
                width, height - (int) Math.Round(halfHeight * circleCornerModifier),
                width - (int) Math.Round(halfWidth * circleCornerModifier), height)
            .CubicBezierTo(
                0, halfHeight,
                (int) Math.Round(halfWidth * circleCornerModifier), height,
                0, height - (int) Math.Round(halfHeight * circleCornerModifier))
            .Close();
    }
}

public static class ColorExt
{
    public static Color AsColor(this string hex)
    {
        if (!(hex.StartsWith('#') && hex.Length is (9 or 7 or 4)))
            throw new ArgumentException("Hex format of color is incorrect.");

        var color = ColorTranslator.FromHtml(hex);

        return color;
    }

    public static Color WithAlpha(this Color color, float alpha)
        => Color.FromArgb((int) (255.0f * alpha), color.R, color.G, color.B);
}