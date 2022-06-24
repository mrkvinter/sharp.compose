namespace SharpCompose.Drawer.Core;

public interface ICanvas
{
    (int w, int h) Size { get; set; }

    void Draw();

    IGraphics StartGraphics();

    void DrawGraphics(int x, int y, IGraphics graphics);
}
