using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Shapes;

namespace SharpCompose.Base.Modifiers.DrawableModifiers;

public sealed class BorderModifier : IDrawableModifier
{
    private readonly int width;
    private readonly Brush brush;
    private readonly IShape shape;

    public BorderModifier(int width, Brush brush, IShape shape)
    {
        this.width = width;
        this.brush = brush;
        this.shape = shape;
    }

    public void Draw(IGraphics graphics, (int w, int h) size, (int x, int y) offset)
        => graphics.StrokeShape(offset, shape, size.w, size.h, width, brush);
}