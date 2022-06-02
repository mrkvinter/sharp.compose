using SharpCompose.Drawer.Core;

namespace SharpCompose.Base.ElementBuilder;

public interface IElementBuilder
{
    (int w, int h) CalculateVisualSize(Composer.Scope scope, ICanvas canvas);

    (int w, int h) CalculateRealSize(Composer.Scope scope, ICanvas canvas);

    void Draw(Composer.Scope scope, ICanvas canvas, int pointerX = 0, int pointerY = 0);
}