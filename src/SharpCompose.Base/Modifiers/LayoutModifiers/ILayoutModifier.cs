using SharpCompose.Base.Layouting;

namespace SharpCompose.Base.Modifiers.LayoutModifiers;

public interface ILayoutModifier : IModifier.IElement
{
    Measurable Introduce(Measurable measurable) =>
        measurable with {Measure = constraints => Measure(measurable, constraints)};


    MeasureResult Measure(Measurable measurable, Constraints constraints);
}