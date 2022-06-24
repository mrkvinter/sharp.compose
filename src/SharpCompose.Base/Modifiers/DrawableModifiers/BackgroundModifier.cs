using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Utilities;

namespace SharpCompose.Base.Modifiers.DrawableModifiers;

public sealed class BackgroundModifier : BaseDrawableModifier
{
    private readonly Brush brush;

    public BackgroundModifier(Brush brush)
    {
        this.brush = brush;
    }

    protected override void Draw(IGraphics graphics, IntSize size, IntOffset offset)
        => graphics.FillRectangle(offset, size, brush);
}