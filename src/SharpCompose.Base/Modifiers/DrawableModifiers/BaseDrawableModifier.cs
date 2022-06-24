using SharpCompose.Base.Layouting;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Utilities;

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
                        Draw(graphics, new IntSize(measureResult.Width, measureResult.Height), new IntOffset(x, y));
                        measureResult.Placeable(x, y);
                    }
                };
            }
        };

    protected abstract void Draw(IGraphics graphics, IntSize size, IntOffset offset);
}