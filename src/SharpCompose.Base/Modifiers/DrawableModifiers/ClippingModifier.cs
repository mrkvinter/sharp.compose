using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Shapes;

namespace SharpCompose.Base.Modifiers.DrawableModifiers;

public sealed class ClippingModifier : BaseDrawableModifier
{
    private readonly IShape shape;

    public ClippingModifier(IShape shape)
    {
        this.shape = shape;
    }

    protected override void Draw(IGraphics graphics, (int w, int h) size, (int x, int y) offset)
        => graphics.Clip(shape, offset, size);
}