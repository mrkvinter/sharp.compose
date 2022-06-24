using System;
using System.Numerics;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Utilities;

namespace SharpCompose.WinUI;

public static class Extensions
{
    public static ICanvasBrush ToCanvasBrush(this Brush brush, ICanvasResourceCreator canvasResourceCreator,
        IntOffset point, IntSize size) => brush switch
    {
        LinearGradientBrush linearGradientBrush => new CanvasLinearGradientBrush(canvasResourceCreator, linearGradientBrush.FirstColor.ToColor(), linearGradientBrush.SecondColor.ToColor())
        {
            StartPoint = new Vector2(point.X + size.Width * linearGradientBrush.FirstPoint.Item1, point.Y + size.Height * linearGradientBrush.FirstPoint.Item2),
            EndPoint = new Vector2(point.X + size.Width * linearGradientBrush.SecondPoint.Item1, point.Y + size.Height * linearGradientBrush.SecondPoint.Item2)
        },

        SolidColorBrush solidColorBrush => new CanvasSolidColorBrush(canvasResourceCreator, solidColorBrush.Color.ToColor()),

        _ => throw new ArgumentOutOfRangeException(nameof(brush))
    };

    public static Color ToColor(this System.Drawing.Color color) => Color.FromArgb(color.A, color.R, color.G, color.B);

    public static CanvasGeometry ToCanvasGeometry(
        this Outline outline,
        ICanvasResourceCreator creator)
    {
        var pathBuilder = new CanvasPathBuilder(creator);

        foreach (var segment in outline.Segments)
        {
            switch (segment)
            {
                case StartSegment startSegment:
                {
                    pathBuilder.BeginFigure(startSegment.X, startSegment.Y);
                    break;
                }
                case LineSegment lineSegment:
                {
                    pathBuilder.AddLine(lineSegment.X, lineSegment.Y);
                    break;
                }
                case CubicBezierSegment cubicBezierSegment:
                {
                    pathBuilder.AddCubicBezier(
                        new Vector2(cubicBezierSegment.FirstControlX, cubicBezierSegment.FirstControlY),
                        new Vector2(cubicBezierSegment.SecondControlX, cubicBezierSegment.SecondControlY),
                        new Vector2(cubicBezierSegment.X, cubicBezierSegment.Y));
                    break;
                }
                case CloseSegment:
                {
                    pathBuilder.EndFigure(CanvasFigureLoop.Closed);
                    break;
                }
            }
        }
        return CanvasGeometry.CreatePath(pathBuilder);
    }
}