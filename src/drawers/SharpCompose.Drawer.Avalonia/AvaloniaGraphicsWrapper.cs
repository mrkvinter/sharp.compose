using Avalonia.Controls;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Images;
using SharpCompose.Drawer.Core.Shapes;
using VectSharp;
using VectSharp.Filters;
using VectSharp.ImageSharpUtils;
using VectSharp.SVG;
using Brush = SharpCompose.Drawer.Core.Brushes.Brush;
using Font = SharpCompose.Drawer.Core.Font;

namespace SharpCompose.Drawer.Avalonia;

public class RasterImage : IImage
{
    public readonly RasterImageStream ImageStream;
    public int Width { get; }

    public int Height { get; }

    public RasterImage(Stream stream)
    {
        ImageStream = new RasterImageStream(stream);
        Width = ImageStream.Width;
        Height = ImageStream.Height;
    }
}

internal sealed class AvaloniaGraphicsWrapper : IGraphics
{
    private Graphics graphics;

    public Graphics Graphics => graphics;

    public AvaloniaGraphicsWrapper(Graphics graphics)
    {
        this.graphics = graphics;
    }

    public void FillRectangle((int x, int y) point, (int w, int h) size, Brush brush)
    {
        graphics.FillRectangle(point.x, point.y, size.w, size.h,
            brush.ToVectBrush(new Rect(point.x, point.y, size.w, size.h)));
    }

    public void StrokeShape((int x, int y) point, IShape shape, int width, int height, int lineWidth, Brush brush)
    {
        var path = shape.CreateOutline(width, height).ToGraphicsPath();
        path = path.Transform(p => new VectSharp.Point(p.X + point.x, p.Y + point.y));

        graphics.StrokePath(path, brush.ToVectBrush(new Rect(point.x, point.y, width, height)), lineWidth);
    }

    public void DrawGraphics(IGraphics otherGraphics, int x, int y)
    {
        var g = ((AvaloniaGraphicsWrapper) otherGraphics).graphics;
        graphics.DrawGraphics(x, y, g);
    }

    public void DrawText(string text, double emSize, Font font, Brush brush, int x, int y)
    {
        if (text.Length == 0)
            return;

        var fontFamily = font.Resolve();
        var vectFont = new VectSharp.Font(fontFamily, emSize);
        var measure = MeasureText(text, emSize, font);
        var rect = new Rect(x, y, measure.w, measure.h);
        graphics.FillText(x, y, text, vectFont, brush.ToVectBrush(rect));
    }

    public void DrawShadow((int x, int y) point, (int w, int h) size, (int x, int y) offset, int blurRadius,
        IShape shape, Brush brush)
    {
        var gaussianBlurFilter = new GaussianBlurFilter(blurRadius);
        var filter = new CompositeLocationInvariantFilter(gaussianBlurFilter);

        var outline = shape.CreateOutline(size.w, size.h).ToGraphicsPath();
        var shadowGraphics = new Graphics();
        var result = new Graphics();
        shadowGraphics.FillPath(outline, brush.ToVectBrush(new Rect(point.x, point.y, size.w, size.h)));
        result.DrawGraphics(point.x + offset.x, point.y + offset.y, shadowGraphics, filter);
        result.DrawGraphics(0, 0, graphics);
        graphics = result;
    }

    public void DrawImage((int x, int y) point, (int w, int h) size, IImage image)
    {
        switch (image)
        {
            case CashedVectorImage cashedVectorImage:
            {
                var scaleFactor = (size.w / (double) cashedVectorImage.Width,
                    size.h / (double) cashedVectorImage.Height);
                var result =
                    cashedVectorImage.CashedGraphics.Transform(
                        p => new Point(p.X * scaleFactor.Item1, p.Y * scaleFactor.Item2), 1);
                graphics.DrawGraphics(point.x, point.y, result);
                break;
            }
            case VectorImage vectorImage:
            {
                var parsedImage = Parser.FromString(vectorImage.SvgSourceSource);
                var scaleFactor = (size.w / parsedImage.Width, size.h / parsedImage.Height);
                var result =
                    parsedImage.Graphics.Transform(p => new Point(p.X * scaleFactor.Item1, p.Y * scaleFactor.Item2), 1);
                graphics.DrawGraphics(point.x, point.y, result);
                break;
            }
            case RasterImage rasterImage:
            {
                (double w, double h) scaleFactor = (size.w / (double)rasterImage.Width, size.h / (double)rasterImage.Height);
                
                if (Math.Abs(scaleFactor.w - 1) > double.Epsilon || Math.Abs(scaleFactor.h - 1) > double.Epsilon)
                    graphics.DrawRasterImage(point.x, point.y, scaleFactor.w, scaleFactor.h, rasterImage.ImageStream);
                else
                    graphics.DrawRasterImage(point.x, point.y, rasterImage.ImageStream);
                
                break;
            }

            default: throw new NotImplementedException();
        }
    }

    public void Clip(IShape shape, (int x, int y) offset, (int w, int h) size)
    {
        var path = shape.CreateOutline(size.w, size.h).ToGraphicsPath();
        path = path.Transform(p => new Point(p.X + offset.x, p.Y + offset.y));

        graphics.SetClippingPath(path);
    }

    public void Clear()
    {
        graphics = new Graphics();
    }

    public (int w, int h) MeasureText(string text, double emSize, Font font)
    {
        var textToCheck = text.Length == 0 ? "1" : text;
        var measuredText = new VectSharp.Font(font.Resolve(), emSize).MeasureTextAdvanced(textToCheck); 

        return (text.Length == 0 ? 0 : (int)measuredText.AdvanceWidth, (int)measuredText.Top);
    }
}