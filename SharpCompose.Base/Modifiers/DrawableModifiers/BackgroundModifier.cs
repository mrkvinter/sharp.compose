using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;

namespace SharpCompose.Base.Modifiers.DrawableModifiers;

public sealed class BackgroundModifier : IDrawableModifier
{
    private readonly Brush brush;

    public BackgroundModifier(Brush brush)
    {
        this.brush = brush;
    }

    public void Draw(IGraphics graphics, (int w, int h) size, (int x, int y) offset)
        => graphics.FillRectangle(offset, size, brush);
}