namespace SharpCompose.Base.Modifiers;


public class Modifier : IScopeModifier<Modifier>
{
    public static Modifier With => new();

    public IModifier SelfModifier { get; set; } = IModifier.Empty;
}
