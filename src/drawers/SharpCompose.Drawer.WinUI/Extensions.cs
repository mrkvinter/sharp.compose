using System;
using System.Numerics;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.UI.Text;
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

    public static Windows.UI.Text.FontWeight ToFontWeight(this FontWeight fontWeight) => fontWeight switch
    {
        {Weight: <= 100} => FontWeights.Thin,
        {Weight: <= 200} => FontWeights.ExtraLight,
        {Weight: <= 300} => FontWeights.Light,
        {Weight: <= 400} => FontWeights.Normal,
        {Weight: <= 500} => FontWeights.Medium,
        {Weight: <= 600} => FontWeights.SemiBold,
        {Weight: <= 700} => FontWeights.Bold,
        {Weight: <= 800} => FontWeights.ExtraBold,
        {Weight: <= 900} => FontWeights.Black,
        _ => throw new ArgumentOutOfRangeException(nameof(fontWeight), fontWeight, null)
    };
}