using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Shapes;
using SharpCompose.Drawer.Core.Utilities;

namespace SharpCompose.Base.Modifiers.DrawableModifiers;

public sealed class BorderModifier : BaseDrawableModifier
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

    protected override void Draw(IGraphics graphics, IntSize size, IntOffset offset)
        => graphics.StrokeShape(offset, shape, size, width, brush);
}