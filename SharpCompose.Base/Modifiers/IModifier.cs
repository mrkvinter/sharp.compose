namespace SharpCompose.Base.Modifiers;

public interface IModifier
{
    public static readonly IModifier Empty = new EmptyModifier();

    IModifier Then(IModifier other) => other == this ? this : new CombinedModifier(this, other);

    IModifier[] SqueezeModifiers();

    public interface IElement : IModifier
    {
        IModifier[] IModifier.SqueezeModifiers() => new IModifier[] {this};
    }

    private sealed class EmptyModifier : IModifier
    {
        public IModifier[] SqueezeModifiers()
        {
            return Array.Empty<IModifier>();
        }
    }
}

public sealed class CombinedModifier : IModifier
{
    private readonly IModifier outer;
    private readonly IModifier inner;

    public CombinedModifier(IModifier outer, IModifier inner)
    {
        this.outer = outer;
        this.inner = inner;
    }

    public IModifier[] SqueezeModifiers()
        => inner.SqueezeModifiers().Concat(outer.SqueezeModifiers()).ToArray();
}

public sealed class DebugModifier : IModifier.IElement
{
    public string? ScopeName { get; init; }
}