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
using SharpCompose.Drawer.Core.Utilities;

namespace SharpCompose.WinUI;

public sealed class WinUIGraphics : IGraphics
{
    private readonly ICanvasResourceCreator canvasResourceCreator;
    private CanvasActiveLayer layer;
    public Action<CanvasDrawingSession> Draw { get; private set; }

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

    public IntSize MeasureText(string text, double emSize, Font font, TextAlignment textAlignment, IntSize maxSize)
    {
        var format = new CanvasTextFormat
        {
            FontSize = (float)emSize,
            FontFamily = font.FontFamily,
            HorizontalAlignment = textAlignment switch
            {
                TextAlignment.Left => CanvasHorizontalAlignment.Left,
                TextAlignment.Center => CanvasHorizontalAlignment.Center,
                TextAlignment.Right => CanvasHorizontalAlignment.Right,
                _ => throw new ArgumentOutOfRangeException(nameof(textAlignment), textAlignment, null)
            }
        };
        using var canvasTextLayout = new CanvasTextLayout(canvasResourceCreator, text, format, maxSize.Width, maxSize.Height);
        return new IntSize((int)canvasTextLayout.LayoutBounds.Width, (int)canvasTextLayout.LayoutBounds.Height);
    }

    public void Clip(IShape shape, IntOffset offset, IntSize size)
    {
        Draw += canvas =>
        {
            layer?.Dispose();
            layer = canvas.CreateLayer(1, shape.CreateOutline(size.Width, size.Height).ToCanvasGeometry(canvas),
                Matrix3x2.CreateTranslation(offset.X, offset.Y));
        };
    }

    public void DrawImage(IntOffset offset, IntSize size, IImage image)
    {
        switch (image)
        {
            case WinUIVectorImage winUIVectorImage:
            {
                Draw += canvas => canvas.DrawSvg(winUIVectorImage.CanvasSvgDocument, new Size(size.Width, size.Height), new Vector2(offset.X, offset.Y));
                break;
            }
        }
    }

    public void DrawShadow(IntOffset offset, IntSize size, IntOffset shadowOffset, int blurRadius,
        IShape shape, Brush brush)
    {
        Draw += canvas =>
        {
            using (var virtualBitmap = new CanvasCommandList(canvas))
            {
                using (var tempCanvas = virtualBitmap.CreateDrawingSession())
                {
                    tempCanvas.FillGeometry(
                        shape.CreateOutline(size.Width, size.Height).ToCanvasGeometry(tempCanvas),
                        0, 0,
                        brush.ToCanvasBrush(tempCanvas, offset, size));
                }

                var shadowEffect = new ShadowEffect
                {
                    Source = virtualBitmap,
                    BlurAmount = blurRadius
                };

                canvas.DrawImage(shadowEffect, offset.X + shadowOffset.X, offset.Y + shadowOffset.Y);
            }
        };
    }

    public void DrawText(string text, double emSize, Font font, Brush brush, IntOffset offset,
        TextAlignment textAlignment, IntSize maxSize)
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
                    HorizontalAlignment = textAlignment switch
                    {
                        TextAlignment.Left => CanvasHorizontalAlignment.Left,
                        TextAlignment.Center => CanvasHorizontalAlignment.Center,
                        TextAlignment.Right => CanvasHorizontalAlignment.Right,
                        _ => throw new ArgumentOutOfRangeException(nameof(textAlignment), textAlignment, null)
                    }
                };
                canvasTextLayout = new CanvasTextLayout(canvas, text, format, maxSize.Width, maxSize.Height);
            }

            
            canvas.DrawTextLayout(canvasTextLayout, 
                offset.X, offset.Y, 
                brush.ToCanvasBrush(canvas, offset, new IntSize((int)canvasTextLayout.LayoutBounds.Width, (int)canvasTextLayout.LayoutBounds.Height)));
        };
    }

    public void FillRectangle(IntOffset offset, IntSize size, Brush brush)
    {
        Draw += canvas => canvas.FillRectangle(offset.X, offset.Y, size.Width, size.Height, brush.ToCanvasBrush(canvas, offset, size));
    }

    public void StrokeShape(IntOffset offset, IShape shape, IntSize size, int lineWidth, Brush brush)
    {
        Draw += canvas => canvas.DrawGeometry(
            shape.CreateOutline(size.Width, size.Height).ToCanvasGeometry(canvas),
            new Vector2(offset.X, offset.Y), brush.ToCanvasBrush(canvas, offset, size),
            lineWidth);
    }
}