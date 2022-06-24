using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Shapes;
using SharpCompose.Drawer.Core.Utilities;

namespace SharpCompose.Base.Modifiers.DrawableModifiers;

public sealed class ShadowModifier : BaseDrawableModifier
{
    private readonly IntOffset shadowOffset;
    private readonly int blurRadius;
    private readonly Brush brush;
    private readonly IShape shape;

    public ShadowModifier(IntOffset shadowOffset, int blurRadius, Brush brush, IShape shape)
    {
        this.shadowOffset = shadowOffset;
        this.blurRadius = blurRadius;
        this.brush = brush;
        this.shape = shape;
    }

    protected override void Draw(IGraphics graphics, IntSize size, IntOffset offset)
        => graphics.DrawShadow(offset, size, shadowOffset, blurRadius, shape, brush);
}
