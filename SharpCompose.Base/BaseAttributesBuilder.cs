namespace SharpCompose.Base;

public abstract class BaseAttributesBuilder
{
    protected readonly Dictionary<string, object> attributes = new();

    public IReadOnlyDictionary<string, object> Attributes => attributes;
}

public interface IAlignment
{
    // size: IntSize,
    // space: IntSize,
    // layoutDirection: LayoutDirection
    (int x, int y) Align((int w, int h) size, (int w, int h) bound);
}

public record BiasAlignment(float HorizontalBias, float VerticalBias) : IAlignment
{
//     val centerX = (space.width - size.width).toFloat() / 2f
//     val centerY = (space.height - size.height).toFloat() / 2f
//     val resolvedHorizontalBias = if (layoutDirection == LayoutDirection.Ltr) {
//         horizontalBias
//     } else {
//     -1 * horizontalBias
// }
//
// val x = centerX * (1 + resolvedHorizontalBias)
// val y = centerY * (1 + verticalBias)
// return IntOffset(x.roundToInt(), y.roundToInt())
    public (int x, int y) Align((int w, int h) size, (int w, int h) bound)
    {
        (float x, float y) center = (
            (bound.w - size.w)/2f, 
            (bound.h - size.h)/2f);

        return ((int)(center.x * (1 + HorizontalBias)), (int) (center.y * (1 + VerticalBias)));
    }
}