namespace SharpCompose.Drawer.Core;

public interface ICanvas
{
    (int w, int h) Size { get; }

    void Draw();

    IGraphics StartGraphics();

    void DrawGraphics(int x, int y, IGraphics graphics);
}
