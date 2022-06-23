namespace SharpCompose.Base;

public interface IAlignment
{
    (int x, int y) Align((int w, int h) parentSize, (int w, int h) size);
}

public record BiasAlignment(float HorizontalBias, float VerticalBias) : IAlignment
{
    public (int x, int y) Align((int w, int h) parentSize, (int w, int h) size)
    {
        (float x, float y) center = (
            (size.w - parentSize.w)/2f, 
            (size.h - parentSize.h)/2f);

        return ((int)(center.x * (1 + HorizontalBias)), (int) (center.y * (1 + VerticalBias)));
    }
}

public static class Alignment
{
    public static readonly IAlignment TopStart = new BiasAlignment(-1, -1);
    public static readonly IAlignment TopCenter = new BiasAlignment(0, -1);
    public static readonly IAlignment TopEnd = new BiasAlignment(1, -1);

    public static readonly IAlignment CenterStart = new BiasAlignment(-1, 0);
    public static readonly IAlignment Center = new BiasAlignment(0, 0);
    public static readonly IAlignment CenterEnd = new BiasAlignment(1, 0);
    
    public static readonly IAlignment BottomStart = new BiasAlignment(-1, 1);
    public static readonly IAlignment BottomCenter = new BiasAlignment(0, 1);
    public static readonly IAlignment BottomEnd = new BiasAlignment(1, 1);
}