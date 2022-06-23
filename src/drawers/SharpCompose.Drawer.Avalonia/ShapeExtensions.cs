using SharpCompose.Drawer.Core;
using VectSharp;
using Font = VectSharp.Font;

namespace SharpCompose.Drawer.Avalonia;

internal static class ShapeExtensions
{
    public static GraphicsPath ToGraphicsPath(this Outline outline)
    {
        var path = new GraphicsPath();

        outline.Segments.ForEach(segment =>
        {
            _ = segment switch
            {
                StartSegment startSegment => path.MoveTo(startSegment.X, startSegment.Y),
                LineSegment lineSegment => path.LineTo(lineSegment.X, lineSegment.Y),
                CloseSegment => path.Close(),
                CubicBezierSegment cbSegment => path.CubicBezierTo(cbSegment.FirstControlX, cbSegment.FirstControlY, cbSegment.SecondControlX, cbSegment.SecondControlY, cbSegment.X, cbSegment.Y),
                _ => throw new ArgumentOutOfRangeException(nameof(segment), segment, null)
            };
        });

        return path;
    }
}

internal static class FontExtension
{
    public static FontFamily Resolve(this Core.Font font)
    {
        return font.FontStream != null ? new FontFamily(font.FontStream) : FontFamily.ResolveFontFamily(font.FontFamily);
    }
}