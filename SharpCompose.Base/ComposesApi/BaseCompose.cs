using System.Drawing;
using SharpCompose.Base.ElementBuilder;

namespace SharpCompose.Base.ComposesApi;

public static class BaseCompose
{
    internal static void VoidScope(Action child)
    {
        Composer.Instance.StartScope(FakeFactory, null, EmptyElementBuilder.Instance);
        child.Invoke();
        Composer.Instance.StopScope();
    }

    public static void Composable(Action content)
    {
        Composer.Instance.StartScope(FakeFactory, null, EmptyElementBuilder.Instance);
        content.Invoke();
        Composer.Instance.StopScope();
    }

    public static void Text(string text, int size = 14, Color? color = default) =>
        TextElement(new TextElementBuilder {Text = text, Size = size, Color = color ?? Color.Black});

    private static void TextElement(
        TextElementBuilder elementBuilder)
    {
        Composer.Instance.StartScope(FakeFactory, null, elementBuilder);
        Composer.Instance.StopScope();
    }

    static void FakeFactory(Composer composer)
    {
    }
}