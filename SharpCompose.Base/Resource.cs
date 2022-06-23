namespace SharpCompose.Base;

public sealed class Resource
{
    public static Resource Instance { get; } = new();

    private readonly Dictionary<string, Func<object>> creators = new();

    private Resource(){}

    public void AddResource<T>(string key, Func<T> streamCreator) where T: class
    {
        creators.Add(key, streamCreator);
    }

    public T GetResource<T>(string key) where T: class
        => (T)creators[key]();
}