namespace SharpCompose.Base.ComposesApi.Providers;

public class LocalAlphaProvider : LocalProvider<float>
{
    private static float DefaultValue => 1;

    static LocalAlphaProvider()
    {
        Provide(DefaultValue).StartProvide();
    }
}