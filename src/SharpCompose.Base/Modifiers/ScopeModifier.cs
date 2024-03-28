namespace SharpCompose.Base.Modifiers;


public class ScopeModifier : IScopeModifier<ScopeModifier>
{
    public IModifier SelfModifier { get; set; } = IModifier.Empty;
    
    public ScopeModifier Then(IModifier modifier)
    {
        SelfModifier = SelfModifier.Then(modifier);

        return this;
    }

    public ScopeModifier Then(ScopeModifier modifier)
        => Then(modifier.SelfModifier);
}
