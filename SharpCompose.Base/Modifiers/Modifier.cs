namespace SharpCompose.Base.Modifiers;


public class Modifier : IScopeModifier<Modifier>
{
    public IModifier SelfModifier { get; set; } = IModifier.Empty;
}
