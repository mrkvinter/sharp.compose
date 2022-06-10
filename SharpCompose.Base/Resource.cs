namespace SharpCompose.Base;

public sealed class Resource
{
    public static Resource Instance { get; } = new();

    private Dictionary<string, Func<Stream>> creators = new();
    private Resource(){}

    public void AddResource(string key, Func<Stream> streamCreator)
    {
        creators.Add(key, streamCreator);
    }

    public Stream GetResource(string key) => creators[key]();
}