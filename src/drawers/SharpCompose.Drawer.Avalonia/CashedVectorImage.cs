using SharpCompose.Drawer.Core.Images;
using VectSharp;
using VectSharp.SVG;

namespace SharpCompose.Drawer.Avalonia;

public sealed class CashedVectorImage : VectorImage
{
    public Graphics CashedGraphics { get; }

    public CashedVectorImage(string svgSource) : base(svgSource)
    {
        var parsed = Parser.FromString(svgSource);
        CashedGraphics.DrawGraphics(0, 0, parsed.Graphics);
        CashedGraphics = parsed.Graphics;
    }
}