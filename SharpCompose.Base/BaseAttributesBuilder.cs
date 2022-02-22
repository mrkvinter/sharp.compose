namespace SharpCompose.Base;

public abstract class BaseAttributesBuilder
{
    protected readonly Dictionary<string, object> attributes = new();

    public IReadOnlyDictionary<string, object> Attributes => attributes;
}