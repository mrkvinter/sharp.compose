namespace SharpCompose.Base.Modifiers;

public interface IScopeModifier<out T> where T : IScopeModifier<T>
{
    public IModifier SelfModifier { get; set; }

    public T Then(IModifier modifier)
    {
        SelfModifier = SelfModifier.Then(modifier);

        return (T)this;
    }
}