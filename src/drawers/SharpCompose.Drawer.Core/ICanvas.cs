using SharpCompose.Drawer.Core.Utilities;

namespace SharpCompose.Drawer.Core;

public interface ICanvas
{
    IntSize Size { get; set; }

    void Draw();

    IGraphics StartGraphics();

    void DrawGraphics(int x, int y, IGraphics graphics);
}
