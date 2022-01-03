using Microsoft.AspNetCore.Components;
using SharpCompose.Base.ElementBuilder;

namespace SharpCompose.Base.ComposesApi;

public static class HtmlCompose
{
    private static void TagElement<T>(
        IElementBuilder elementBuilder,
        Action<T>? attributes,
        Action? child) where T : BaseAttributesBuilder, new()
    {
        void Factory(Composer composer)
        {
            var builder = new T();
            attributes?.Invoke(builder);
            composer.BuildAttributes(builder.Attributes);
        }

        Composer.Instance.StartScope(Factory, elementBuilder);
        child?.Invoke();
        Composer.Instance.StopScope();
    }

    public static void Div(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Div, attributes, child);

    public static void H1(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.H1, attributes, child);

    public static void P(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.P, attributes, child);

    public static void A(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.A, attributes, child);

    public static void Span(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Span, attributes, child);

    public static void Nav(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Nav, attributes, child);

    public static void Button(
        Action? onClick = default,
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement<CommonTagAttributesBuilder>(TagElementBuilder.Button, atr =>
    {
        if (onClick != null) atr.OnClick(onClick);
        attributes?.Invoke(atr);
    }, child);

    public static void Em(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Em, attributes, child);

    public static void Table(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Table, attributes, child);

    public static void Thead(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Thead, attributes, child);

    public static void Tr(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Tr, attributes, child);

    public static void Th(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Th, attributes, child);

    public static void Td(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Td, attributes, child);

    public static void Tbody(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Tbody, attributes, child);

    public static void Ul(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Ul, attributes, child);

    public static void Li(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Li, attributes, child);

    public static void Article(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Article, attributes, child);

    public static void Main(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.Main, attributes, child);

    public static void I(
        Action<CommonTagAttributesBuilder>? attributes = default,
        Action? child = default) => TagElement(TagElementBuilder.I, attributes, child);
}