namespace SharpCompose.Base.Modifiers;

//todo: or remove or using make more useful
public interface IScopeModifier<out T> where T : IScopeModifier<T>
{
    public IModifier SelfModifier { get; set; }

    T Then(IModifier modifier);
}