using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Shapes;

namespace SharpCompose.Base.Modifiers.DrawableModifiers;

public sealed class ShadowModifier : IDrawableModifier
{
    private readonly (int x, int y) shadowOffset;
    private readonly int blurRadius;
    private readonly Brush brush;
    private readonly IShape shape;

    public ShadowModifier((int x, int y) shadowOffset, int blurRadius, Brush brush, IShape shape)
    {
        this.shadowOffset = shadowOffset;
        this.blurRadius = blurRadius;
        this.brush = brush;
        this.shape = shape;
    }

    public void Draw(IGraphics graphics, (int w, int h) size, (int x, int y) offset)
        => graphics.DrawShadow(offset, size, shadowOffset, blurRadius, shape, brush);
}
