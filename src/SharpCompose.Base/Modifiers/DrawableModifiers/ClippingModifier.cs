using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Shapes;
using SharpCompose.Drawer.Core.Utilities;

namespace SharpCompose.Base.Modifiers.DrawableModifiers;

public sealed class ClippingModifier : BaseDrawableModifier
{
    private readonly IShape shape;

    public ClippingModifier(IShape shape)
    {
        this.shape = shape;
    }

    protected override void Draw(IGraphics graphics, IntSize size, IntOffset offset)
        => graphics.Clip(shape, offset, size);
}