using System;
using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Text;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Images;
using SharpCompose.Drawer.Core.Shapes;
using Windows.Foundation;

namespace SharpCompose.WinUI;

public sealed class WinUIGraphics : IGraphics
{
    private readonly ICanvasResourceCreator canvasResourceCreator;
    private CanvasActiveLayer layer;
    public Action<CanvasDrawingSession> Draw;

    public WinUIGraphics(ICanvasResourceCreator canvasResourceCreator)
    {
        this.canvasResourceCreator = canvasResourceCreator;
        Draw = _ => { };
    }


    public void Clear()
    {
        Draw = _ => { };
    }

    public void Pop()
    {
        layer?.Dispose();
        layer = null;
    }

    public (int w, int h) MeasureText(string text, double emSize, Font font)
    {
        var format = new CanvasTextFormat
        {
            FontSize = (float)emSize,
            FontFamily = font.FontFamily,
        };
        using var canvasTextLayout = new CanvasTextLayout(canvasResourceCreator, text, format, float.MaxValue, float.MaxValue);

        return ((int)canvasTextLayout.LayoutBounds.Width, (int)canvasTextLayout.LayoutBounds.Height);
    }

    public void Clip(IShape shape, (int x, int y) offset, (int w, int h) size)
    {
        Draw += canvas =>
        {
            layer?.Dispose();
            layer = canvas.CreateLayer(1, shape.CreateOutline(size.w, size.h).ToCanvasGeometry(canvas),
                Matrix3x2.CreateTranslation(offset.x, offset.y));
        };
    }

    public void DrawGraphics(IGraphics otherGraphics, int x, int y)
    {
        throw new NotImplementedException();
    }

    public void DrawImage((int x, int y) point, (int w, int h) size, IImage image)
    {
        switch (image)
        {
            case WinUIVectorImage winUIVectorImage:
            {
                Draw += canvas => canvas.DrawSvg(winUIVectorImage.CanvasSvgDocument, new Size(size.w, size.h), new Vector2(point.x, point.y));
                break;
            }
        }
    }

    public void DrawShadow((int x, int y) point, (int w, int h) size, (int x, int y) offset, int blurRadius,
        IShape shape, Brush brush)
    {
        Draw += canvas =>
        {
            using (var virtualBitmap = new CanvasCommandList(canvas))
            {
                using (var tempCanvas = virtualBitmap.CreateDrawingSession())
                {
                    tempCanvas.FillGeometry(
                        shape.CreateOutline(size.w, size.h).ToCanvasGeometry(tempCanvas),
                        0, 0,
                        brush.ToCanvasBrush(tempCanvas, point, size));
                }

                var shadowEffect = new ShadowEffect
                {
                    Source = virtualBitmap,
                    BlurAmount = blurRadius
                };

                canvas.DrawImage(shadowEffect, point.x + offset.x, point.y + offset.y);
            }
        };
    }

    public void DrawText(string text, double emSize, Font font, Brush brush, int x, int y)
    {
        CanvasTextLayout canvasTextLayout = null;
        Draw += canvas =>
        {
            if (canvasTextLayout == null)
            {
                var format = new CanvasTextFormat
                {
                    FontSize = (float)emSize,
                    FontFamily = font.FontFamily,
                };
                canvasTextLayout = new CanvasTextLayout(canvas, text, format, float.MaxValue, float.MaxValue);
            }

            
            canvas.DrawTextLayout(canvasTextLayout, 
                x, y, 
                brush.ToCanvasBrush(canvas, (x, y), ((int)canvasTextLayout.LayoutBounds.Width, (int)canvasTextLayout.LayoutBounds.Height)));
        };
    }

    public void FillRectangle((int x, int y) point, (int w, int h) size, Brush brush)
    {
        Draw += canvas => canvas.FillRectangle(point.x, point.y, size.w, size.h, brush.ToCanvasBrush(canvas, point, size));
    }

    public void StrokeShape((int x, int y) point, IShape shape, int width, int height, int lineWidth, Brush brush)
    {
        Draw += canvas => canvas.DrawGeometry(
            shape.CreateOutline(width, height).ToCanvasGeometry(canvas),
            new Vector2(point.x, point.y), brush.ToCanvasBrush(canvas, point, (width, height)),
            lineWidth);
    }
}