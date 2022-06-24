namespace SharpCompose.Drawer.Core.Utilities;

public record struct IntOffset(int X, int Y)
{
    public static readonly IntOffset Zero = new(0, 0);

    public static implicit operator IntOffset((int, int) tuple) => new(tuple.Item1, tuple.Item2);
}