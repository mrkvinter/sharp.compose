using SharpCompose.Base.Extensions;
using SharpCompose.Drawer.Core;

namespace SharpCompose.Base.Layouting;

public record BoxParentData(bool MatchParentWidth, bool MatchParentHeight) : IParentData
{
    public bool MatchParentSize => MatchParentWidth || MatchParentHeight;
}

public static class BoxLayout
{
    public static Measure Measure(IAlignment alignment) => (measures, constraints) =>
    {
        var (width, height) = (0, 0);
        var childConstraints = new Constraints(0, constraints.MaxWidth, 0, constraints.MaxHeight);
        var placeables = new MeasureResult[measures.Length];
        var isMatchParentSize = false;
        foreach (var (index, measure) in measures.Indexed())
        {
            if (measure.ParentData is BoxParentData {MatchParentSize: true})
                isMatchParentSize = true;
            else
            {
                var measureResult = measure.Measure(childConstraints);
                placeables[index] = measureResult;
                (width, height) = (
                    Math.Max(width, measureResult.Width),
                    Math.Max(height, measureResult.Height));
            }
        }

        (width, height) = (constraints.ClampWidth(width), constraints.ClampHeight(height));

        if (isMatchParentSize)
        {
            foreach (var (index, measurable) in measures.Indexed())
            {
                if (measurable.ParentData is not BoxParentData {MatchParentSize: true} boxParentData) continue;

                var (minWidth, maxWidth) = boxParentData.MatchParentWidth
                    ? (width, width)
                    : (childConstraints.MinWidth, childConstraints.MaxWidth);

                var (minHeight, maxHeight) = boxParentData.MatchParentHeight
                    ? (height, height)
                    : (childConstraints.MinHeight, childConstraints.MaxHeight);

                var measureResult =
                    measurable.Measure(new Constraints(minWidth, maxWidth, minHeight, maxHeight));

                placeables[index] = measureResult;
            }
        }

        return new MeasureResult
        {
            Width = width,
            Height = height,
            Placeable = (x, y) => placeables.ForEach(placeable =>
            {
                var offset = alignment.Align((width, height), (placeable.Width, placeable.Height));

                placeable.Placeable(x - offset.x, y - offset.y);
            })
        };
    };
}