namespace SharpCompose.Base.Layouting;

public struct Constraints
{
    private const int Infinity = int.MaxValue;

    public readonly int MinWidth = 0;
    public readonly int MaxWidth = Infinity;
    public readonly int MinHeight = 0;
    public readonly int MaxHeight = Infinity;

    public Constraints(int minWidth, int maxWidth, int minHeight, int maxHeight)
    {
        if (!(minWidth >= 0 && minHeight >= 0))
            throw new ArgumentOutOfRangeException($"minWidth({minWidth}) and minHeight({minHeight}) must be >= 0");

        if (!(maxWidth >= minWidth || maxWidth == Infinity))
            throw new ArgumentOutOfRangeException($"maxWidth({maxWidth}) must be >= minWidth({minHeight})");

        if (!(maxHeight >= minHeight || maxHeight == Infinity))
            throw new ArgumentOutOfRangeException($"maxHeight({maxHeight}) must be >= minHeight({minHeight})");

        MinWidth = minWidth;
        MaxWidth = maxWidth;
        MinHeight = minHeight;
        MaxHeight = maxHeight;
    }

    public Constraints Offset(int horizontal, int vertical)
    {
        var maxWidth = MaxWidth == Infinity ? MaxWidth : MaxWidth + horizontal;
        var maxHeight = MaxHeight == Infinity ? MaxHeight : MaxHeight + vertical;
        var minWidth = MinWidth + horizontal > 0 ? MinWidth + horizontal : 0;
        var minHeight = MinHeight + vertical > 0 ? MinHeight + vertical : 0;

        return new Constraints(
            minWidth, maxWidth,
            minHeight, maxHeight);
    }

    public override string ToString() =>
        $"Constraints(minWidth = {MinWidth}, maxWidth = {(MaxWidth == Infinity ? "Infinity" : MaxWidth)}, " +
        $"minHeight = {MinHeight}, maxHeight = {(MaxHeight == Infinity ? "Infinity" : MaxHeight)})";

    public static Constraints Fixed(int width, int height)
        => new(width, width, height, height);

    public static Constraints FixedWidth(int width)
        => new(width, width, 0, Infinity);

    public static Constraints FixedHeight(int height)
        => new(0, Infinity, height, height);

    public static Constraints MinSize(int width, int height)
        => new(width, Infinity, height, Infinity);
}