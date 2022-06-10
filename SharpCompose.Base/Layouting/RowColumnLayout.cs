using SharpCompose.Base.Extensions;

namespace SharpCompose.Base.Layouting;

public enum Direction
{
    Vertical,
    Horizontal
}

public static class RowColumnLayout
{
    public static Measure MeasureColumn(IAlignment crossAxisAlignment)
    {
        return (measures, constraints)
            => MeasureLayout(Direction.Vertical, crossAxisAlignment, measures, constraints);
    }

    public static Measure MeasureRow(IAlignment crossAxisAlignment)
    {
        return (measures, constraints)
            => MeasureLayout(Direction.Horizontal, crossAxisAlignment, measures, constraints);
    }

    private static MeasureResult MeasureLayout(
        Direction direction,
        IAlignment crossAxisAlignment,
        Measurable[] measures,
        Constraints constraints)
    {
        var (width, height) = (0, 0);
        var childConstraints = new Constraints(0, constraints.MaxWidth, 0, constraints.MaxHeight);
        var placeables = new MeasureResult[measures.Length];
        foreach (var (index, measurable) in measures.Indexed())
        {
            var measureResult = measurable.Measure(childConstraints);
            placeables[index] = measureResult;
            (width, height) = MeasureSize(direction, (width, height),
                (measureResult.Width, measureResult.Height));
        }

        (width, height) = (constraints.ClampWidth(width), constraints.ClampHeight(height));

        return new MeasureResult
        {
            Width = width,
            Height = height,
            Placeable = (x, y) =>
            {
                foreach (var placeable in placeables)
                {
                    var (alignX, alignY) = direction == Direction.Vertical
                        ? crossAxisAlignment.Align((width, 0), (placeable.Width, 0))
                        : crossAxisAlignment.Align((0, height), (0, placeable.Height));

                    placeable.Placeable(x - alignX, y - alignY);

                    var (shiftX, shiftY) = MeasureShiftPointer(direction,
                        (placeable.Width, placeable.Height));

                    (x, y) = (x + shiftX, y + shiftY);
                }
            }
        };
    }

    private static (int w, int h)
        MeasureSize(Direction direction, (int w, int h) currentSize, (int w, int h) childSize) =>
        (CalculateWidth(direction, currentSize, childSize),
            CalculateHeight(direction, currentSize, childSize));

    private static (int x, int y) MeasureShiftPointer(Direction direction,
        (int w, int h) childSize) =>
        direction switch
        {
            Direction.Vertical => (0, childSize.h),

            Direction.Horizontal => (childSize.w, 0),

            _ => throw new NotImplementedException()
        };

    private static int CalculateWidth(Direction direction, (int w, int h) currentSize, (int w, int h) childSize)
    {
        return direction switch
        {
            Direction.Vertical => Math.Max(currentSize.w, childSize.w),

            Direction.Horizontal => currentSize.w + childSize.w,

            _ => throw new NotImplementedException()
        };
    }

    private static int CalculateHeight(Direction direction, (int w, int h) currentSize, (int w, int h) childSize)
    {
        return direction switch
        {
            Direction.Vertical => currentSize.h + childSize.h,

            Direction.Horizontal => Math.Max(currentSize.h, childSize.h),

            _ => throw new NotImplementedException()
        };
    }
}