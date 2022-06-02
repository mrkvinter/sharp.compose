using SharpCompose.Drawer.Core;

namespace SharpCompose.Base.ElementBuilder;

public sealed class EmptyElementBuilder : IElementBuilder
{
    public static EmptyElementBuilder Instance { get; } = new();

    private EmptyElementBuilder()
    {
    }

    public (int w, int h) CalculateVisualSize(Composer.Scope scope, ICanvas canvas) => (0, 0);

    public (int w, int h) CalculateRealSize(Composer.Scope scope, ICanvas canvas) => (0, 0);

    public void Draw(Composer.Scope scope, ICanvas canvas, int pointerX, int pointerY)
    {
    }
}