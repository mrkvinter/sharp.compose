using SharpCompose.Base.Layouting;

namespace SharpCompose.Base.Modifiers.LayoutModifiers;

public sealed class FillModifier : ILayoutModifier
{
    private readonly Direction direction;

    public FillModifier(Direction direction)
    {
        this.direction = direction;
    }

    public MeasureResult Measure(Measurable measurable, Constraints constraints)
    {
        if (direction == Direction.Horizontal)
        {
            var width = constraints.MaxWidth;
            constraints = new Constraints(width, width, constraints.MinHeight, constraints.MaxHeight);
        }

        if (direction == Direction.Vertical)
        {
            var height = constraints.MaxHeight;
            constraints = new Constraints(constraints.MinWidth, constraints.MaxWidth, height, height);
        }

        return measurable.Measure(constraints);
    }
}