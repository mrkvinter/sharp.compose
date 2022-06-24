using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Utilities;

namespace SharpCompose.Base.Modifiers.DrawableModifiers;

public sealed class DrawableModifier : BaseDrawableModifier
{
    private readonly Action<IGraphics, IntSize, IntOffset> draw;
    public DrawableModifier(Action<IGraphics, IntSize, IntOffset> draw)
    {
        this.draw = draw;
    }

    protected override void Draw(IGraphics graphics, IntSize size, IntOffset offset) => draw(graphics,
        new IntSize(size.Width, size.Height), new IntOffset(offset.X, offset.Y));
}