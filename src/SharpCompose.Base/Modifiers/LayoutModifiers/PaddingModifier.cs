using SharpCompose.Base.Layouting;

namespace SharpCompose.Base.Modifiers.LayoutModifiers;

public sealed class PaddingModifier : ILayoutModifier
{
    private readonly int start;
    private readonly int top;
    private readonly int end;
    private readonly int bottom;

    public PaddingModifier(int start, int top, int end, int bottom)
    {
        this.start = start;
        this.top = top;
        this.end = end;
        this.bottom = bottom;
    }

    public MeasureResult Measure(Measurable measurable, Constraints constraints)
    {
        var horizontal = start + end;
        var vertical = top + bottom;

        var measureResult = measurable.Measure(constraints.Offset(-horizontal, -vertical));

        var width = measureResult.Width + horizontal;
        var height = measureResult.Height + vertical;

        return new MeasureResult
        {
            Width = width,
            Height = height,
            Placeable = (x, y) => measureResult.Placeable(x + start, y + top)
        };
    }
}