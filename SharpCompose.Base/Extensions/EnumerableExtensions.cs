namespace SharpCompose.Base.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<(int index, T e)> Indexed<T>(this IEnumerable<T> enumerable)
    {
        var index = 0;
        foreach (var e in enumerable)
        {
            yield return (index++, e);
        }
    }
}
