using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Images;
using SharpCompose.Drawer.Core.Shapes;

namespace SharpCompose.Drawer.Core;

public interface IGraphics
{
    void FillRectangle((int x, int y) point, (int w, int h) size, Brush brush);

    void StrokeShape((int x, int y) point, IShape shape, int width, int height, int lineWidth, Brush brush);

    void DrawGraphics(IGraphics otherGraphics, int x, int y);

    void DrawText(string text, double emSize, Font font, Brush brush, int x, int y);

    void DrawShadow((int x, int y) point, (int w, int h) size, (int x, int y) offset, int blurRadius, IShape shape, Brush brush);

    void DrawImage((int x, int y) point, (int w, int h) size, IImage image);

    (int w, int h) MeasureText(string text, double emSize, Font font);

    void Clip(IShape shape, (int x, int y) offset, (int w, int h) size);

    void Clear();
}