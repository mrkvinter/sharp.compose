namespace SharpCompose.Base.Layouting;

public readonly record struct Measurable(Func<Constraints, MeasureResult> Measure, IParentData? ParentData = null);

public struct MeasureResult
{
    public int Width { get; init; }

    public int Height { get; init; }

    public Action<int, int> Placeable { get; init; }
}