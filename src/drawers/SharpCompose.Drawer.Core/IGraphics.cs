using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Images;
using SharpCompose.Drawer.Core.Shapes;
using SharpCompose.Drawer.Core.Utilities;

namespace SharpCompose.Drawer.Core;

public interface IGraphics
{
    void FillRectangle(IntOffset offset, IntSize size, Brush brush);

    void StrokeShape(IntOffset offset, IShape shape, IntSize size, int lineWidth, Brush brush);

    void DrawText(string text, double emSize, Font font, Brush brush, IntOffset offset, TextAlignment textAlignment,
        IntSize maxSize);

    void DrawShadow(IntOffset offset, IntSize size, IntOffset shadowOffset, int blurRadius, IShape shape, Brush brush);

    void DrawImage(IntOffset offset, IntSize size, IImage image);

    IntSize MeasureText(string text, double emSize, Font font, TextAlignment textAlignment, IntSize maxSize);

    void Clip(IShape shape, IntOffset offset, IntSize size);

    void Clear();
}