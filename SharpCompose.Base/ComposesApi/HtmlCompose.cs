using SharpCompose.Base.ElementBuilder;

namespace SharpCompose.Base.ComposesApi;

public static class HtmlCompose
{
    private static void TagElement(
        IElementBuilder elementBuilder,
        Action<TagAttributesBuilder>? attributes,
        Action? child)
    {
        void Factory(Composer composer)
        {
            var builder = new TagAttributesBuilder();
            attributes?.Invoke(builder);
            builder.Build(composer);
        }

        Composer.Instance.StartScope(Factory, child, elementBuilder);
        child?.Invoke();
        Composer.Instance.StopScope();
    }

    public static void Div(
        Action<TagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Div, attributes, child);

    public static void H1(
        Action<TagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.H1, attributes, child);

    public static void P(
        Action<TagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.P, attributes, child);

    public static void A(
        Action<TagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.A, attributes, child);

    public static void Span(
        Action<TagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Span, attributes, child);

    public static void Nav(
        Action<TagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Nav, attributes, child);

    public static void Button(
        Action<TagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Button, attributes, child);

    public static void Em(
        Action<TagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Em, attributes, child);

    public static void Table(
        Action<TagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Table, attributes, child);

    public static void Thead(
        Action<TagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Thead, attributes, child);

    public static void Tr(
        Action<TagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Tr, attributes, child);

    public static void Th(
        Action<TagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Th, attributes, child);

    public static void Td(
        Action<TagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Td, attributes, child);

    public static void Tbody(
        Action<TagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Tbody, attributes, child);
}