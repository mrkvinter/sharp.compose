using System.Xml.Linq;
using static System.Int32;

namespace SharpCompose.Drawer.Core.Images;

public interface IImage
{
    int Width { get; }
    int Height { get; }
}

public class VectorImage : IImage
{
    private readonly string svgSource;
    
    public string SvgSourceSource => svgSource;

    public int Width { get; }
    public int Height { get; }

    public VectorImage(string svgSource)
    {
        this.svgSource = svgSource;

        (Width, Height) = ParseSize();
    }

    private (int, int) ParseSize()
    {
        var width = 0;
        var height = 0;
        var document = XDocument.Parse(svgSource);
        if (document.Root != null)
        {
            var widthAttribute = document.Root.Attribute("width");
            var heightAttribute = document.Root.Attribute("height");
            TryParse(widthAttribute?.Value, out width);
            TryParse(heightAttribute?.Value, out height);
        }

        return (width, height);
    }
}