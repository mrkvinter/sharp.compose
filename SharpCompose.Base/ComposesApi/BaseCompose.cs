using SharpCompose.Base.ElementBuilder;

namespace SharpCompose.Base.ComposesApi;

public static class BaseCompose
{
    internal static void VoidScope(Action child)
    {
        Composer.Instance.StartScope(FakeFactory);
        child.Invoke();
        Composer.Instance.StopScope();
    }

    public static void Text(string text) => TextElement(new TextElementBuilder {Text = text});

    private static void TextElement(
        TextElementBuilder elementBuilder)
    {
        Composer.Instance.StartScope(FakeFactory, elementBuilder);
        Composer.Instance.StopScope();
    }

    static void FakeFactory(Composer composer)
    {
    }
}