using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Images;
using SharpCompose.Drawer.Core.Shapes;
using SharpCompose.Drawer.Core.Utilities;
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

    public void FillRectangle(IntOffset offset, IntSize size, Brush brush)
    {
        graphics.FillRectangle(offset.X, offset.Y, size.Width, size.Height,
            brush.ToVectBrush(new Rect(offset.X, offset.Y, size.Width, size.Height)));
    }

    public void StrokeShape(IntOffset offset, IShape shape, IntSize size, int lineWidth, Brush brush)
    {
        var path = shape.CreateOutline(size.Width, size.Height).ToGraphicsPath();
        path = path.Transform(p => new Point(p.X + offset.X, p.Y + offset.Y));

        graphics.StrokePath(path, brush.ToVectBrush(new Rect(offset.X, offset.Y, size.Width, size.Height)), lineWidth);
    }

    public void DrawText(string text, double emSize, Font font, Brush brush, IntOffset offset)
    {
        if (text.Length == 0)
            return;

        var fontFamily = font.Resolve();
        var vectFont = new VectSharp.Font(fontFamily, emSize);
        var measure = MeasureText(text, emSize, font);
        var rect = new Rect(offset.X, offset.Y, measure.Width, measure.Height);
        graphics.FillText(offset.X, offset.Y, text, vectFont, brush.ToVectBrush(rect));
    }

    public void DrawShadow(IntOffset offset, IntSize size, IntOffset shadowOffset, int blurRadius,
        IShape shape, Brush brush)
    {
        var gaussianBlurFilter = new GaussianBlurFilter(blurRadius);
        var filter = new CompositeLocationInvariantFilter(gaussianBlurFilter);

        var outline = shape.CreateOutline(size.Width, size.Height).ToGraphicsPath();
        var shadowGraphics = new Graphics();
        var result = new Graphics();
        shadowGraphics.FillPath(outline, brush.ToVectBrush(new Rect(offset.X, offset.Y, size.Width, size.Height)));
        result.DrawGraphics(offset.X + shadowOffset.X, offset.Y + shadowOffset.Y, shadowGraphics, filter);
        result.DrawGraphics(0, 0, graphics);
        graphics = result;
    }

    public void DrawImage(IntOffset offset, IntSize size, IImage image)
    {
        switch (image)
        {
            case CashedVectorImage cashedVectorImage:
            {
                var scaleFactor = (size.Width / (double) cashedVectorImage.Width,
                    size.Height / (double) cashedVectorImage.Height);
                var result =
                    cashedVectorImage.CashedGraphics.Transform(
                        p => new Point(p.X * scaleFactor.Item1, p.Y * scaleFactor.Item2), 1);
                graphics.DrawGraphics(offset.X, offset.Y, result);
                break;
            }
            case VectorImage vectorImage:
            {
                var parsedImage = Parser.FromString(vectorImage.SvgSourceSource);
                var scaleFactor = (size.Width / parsedImage.Width, size.Height / parsedImage.Height);
                var result =
                    parsedImage.Graphics.Transform(p => new Point(p.X * scaleFactor.Item1, p.Y * scaleFactor.Item2), 1);
                graphics.DrawGraphics(offset.X, offset.Y, result);
                break;
            }
            case RasterImage rasterImage:
            {
                (double w, double h) scaleFactor = (size.Width / (double)rasterImage.Width, size.Height / (double)rasterImage.Height);
                
                if (Math.Abs(scaleFactor.w - 1) > double.Epsilon || Math.Abs(scaleFactor.h - 1) > double.Epsilon)
                    graphics.DrawRasterImage(offset.X, offset.Y, scaleFactor.w, scaleFactor.h, rasterImage.ImageStream);
                else
                    graphics.DrawRasterImage(offset.X, offset.Y, rasterImage.ImageStream);
                
                break;
            }

            default: throw new NotImplementedException();
        }
    }

    public void Clip(IShape shape, IntOffset offset, IntSize size)
    {
        var path = shape.CreateOutline(size.Width, size.Height).ToGraphicsPath();
        path = path.Transform(p => new Point(p.X + offset.X, p.Y + offset.Y));

        graphics.SetClippingPath(path);
    }

    public void Clear()
    {
        graphics = new Graphics();
    }

    public IntSize MeasureText(string text, double emSize, Font font)
    {
        var textToCheck = text.Length == 0 ? "1" : text;
        var measuredText = new VectSharp.Font(font.Resolve(), emSize).MeasureTextAdvanced(textToCheck); 

        return new IntSize(text.Length == 0 ? 0 : (int)measuredText.AdvanceWidth, (int)measuredText.Top);
    }
}