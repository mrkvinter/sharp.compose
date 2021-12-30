using SharpCompose.Base.ElementBuilder;

namespace SharpCompose.Base.ComposesApi;

public static class BaseCompose
{
    public static void Text(string text) => TextElement(new TextElementBuilder {Text = text});

    private static void TextElement(
        TextElementBuilder elementBuilder)
    {
        
        static void FakeFactory(Composer composer)
        {
        }

        Composer.Instance.StartScope(FakeFactory, elementBuilder);
        Composer.Instance.StopScope();
    }
}
