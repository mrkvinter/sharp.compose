using SharpCompose.Base.Utilities;
using SharpCompose.Drawer.Core;

namespace SharpCompose.Base.Modifiers.DrawableModifiers;

public sealed class DrawableModifier : BaseDrawableModifier
{
    private readonly Action<IGraphics, IntSize, IntVector2> draw;
    public DrawableModifier(Action<IGraphics, IntSize, IntVector2> draw)
    {
        this.draw = draw;
    }

    protected override void Draw(IGraphics graphics, (int w, int h) size, (int x, int y) offset) => draw(graphics,
        new IntSize(size.w, size.h), new IntVector2(offset.x, offset.y));
}