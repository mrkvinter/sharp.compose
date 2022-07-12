namespace SharpCompose.Base.Extensions;

public static class UtilsExtensions
{
    public static MutableState<T> AsMutableState<T>(this T self) where T : notnull => new(self);
}