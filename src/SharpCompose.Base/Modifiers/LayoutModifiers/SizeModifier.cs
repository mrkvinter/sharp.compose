using SharpCompose.Base.Layouting;

namespace SharpCompose.Base.Modifiers.LayoutModifiers;

public sealed class SizeModifier : ILayoutModifier
{
    private readonly Constraints localConstraints;

    public SizeModifier(Constraints constraints)
    {
        localConstraints = constraints;
    }

    public MeasureResult Measure(Measurable measurable, Constraints constraints)
    {
        var wrappedConstraints = new Constraints(
            Math.Clamp(localConstraints.MinWidth, constraints.MinWidth, constraints.MaxWidth),
            Math.Clamp(localConstraints.MaxWidth, constraints.MinWidth, constraints.MaxWidth),
            Math.Clamp(localConstraints.MinHeight, constraints.MinHeight, constraints.MaxHeight),
            Math.Clamp(localConstraints.MaxHeight, constraints.MinHeight, constraints.MaxHeight));

        return measurable.Measure(wrappedConstraints);
    }
}