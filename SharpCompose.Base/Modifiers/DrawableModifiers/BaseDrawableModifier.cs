using SharpCompose.Base.Layouting;
using SharpCompose.Drawer.Core;

namespace SharpCompose.Base.Modifiers.DrawableModifiers;

public abstract class BaseDrawableModifier : IDrawableLayerModifier
{
    public Measurable Introduce(Measurable measurable, IGraphics graphics) =>
        measurable with
        {
            Measure = constraints =>
            {
                var measureResult = measurable.Measure(constraints);

                return measureResult with
                {
                    Placeable = (x, y) =>
                    {
                        Draw(graphics, (measureResult.Width, measureResult.Height), (x, y));
                        measureResult.Placeable(x, y);
                    }
                };
            }
        };

    protected abstract void Draw(IGraphics graphics, (int w, int h) size, (int x, int y) offset);
}