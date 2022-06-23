namespace SharpCompose.Drawer.Core;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> callback)
    {
        foreach (var e in enumerable)
        {
            callback(e);
        }
    }
}