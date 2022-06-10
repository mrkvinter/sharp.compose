using SharpCompose.Base.Layouting;

namespace SharpCompose.Base.Modifiers;

public interface IParentDataModifier : IModifier.IElement
{
    Measurable Introduce(Measurable measurable);
}

public class ParentDataModifier<T> : IParentDataModifier
    where T : class, IParentData
{
    private readonly Func<T?, T> parentDataModifier;

    public ParentDataModifier(Func<T?, T> parentDataModifier)
    {
        this.parentDataModifier = parentDataModifier;
    }

    public Measurable Introduce(Measurable measurable) =>
        measurable with {ParentData = parentDataModifier(measurable.ParentData as T)};
}