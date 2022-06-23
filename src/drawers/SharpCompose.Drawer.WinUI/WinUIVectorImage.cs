using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Svg;
using SharpCompose.Drawer.Core.Images;

namespace SharpCompose.WinUI;

public sealed class WinUIVectorImage : IImage
{
    public int Width { get; }

    public int Height { get; }

    public readonly CanvasSvgDocument CanvasSvgDocument;

    public WinUIVectorImage(ICanvasResourceCreator resourceCreator, string svg)
    {
        CanvasSvgDocument = CanvasSvgDocument.LoadFromXml(resourceCreator, svg);
        Width = (int)CanvasSvgDocument.Root.GetFloatAttribute("width");
        Height = (int)CanvasSvgDocument.Root.GetFloatAttribute("height");
    }
}