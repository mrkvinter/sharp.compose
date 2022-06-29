using SharpCompose.Base.Layouting;
using SharpCompose.Base.Modifiers.Extensions;
using SharpCompose.Base.Modifiers.LayoutModifiers;

namespace SharpCompose.Base.Modifiers;

public interface IInputModifier : ILayoutModifier
{
}

public record struct MouseState(bool IsOver);

public sealed class OnMouseInputModifier : IInputModifier
{
    private readonly MutableState<BoundState> boundState;

    public Action? OnMouseOver { get; init; }
    public Action? OnMouseOut { get; init; }
    public Action? OnMouseDown { get; init; }
    public Action? OnMouseUp { get; init; }

    public OnMouseInputModifier(MutableState<BoundState> boundState)
    {
        this.boundState = boundState;
    }

    public MeasureResult Measure(Measurable measurable, Constraints constraints)
    {
        var measureResult = measurable.Measure(constraints);

        return measureResult with
        {
            Placeable = (x, y) =>
            {
                boundState.Value = new BoundState(x, y, measureResult.Width, measureResult.Height);
                measureResult.Placeable(x, y);
            }
        };
    }
}