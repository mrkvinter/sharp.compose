namespace SharpCompose.Base.Modifiers;


public class ScopeModifier : IScopeModifier<ScopeModifier>
{
    public IModifier SelfModifier { get; set; } = IModifier.Empty;
}
