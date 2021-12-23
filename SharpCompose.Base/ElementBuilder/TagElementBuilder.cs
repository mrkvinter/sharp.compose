namespace SharpCompose.Base.ElementBuilder;

internal class TagElementBuilder : IElementBuilder
{
    public string Tag { get; }

    private TagElementBuilder(string tag)
    {
        Tag = tag;
    }

    private static IElementBuilder CreateBuilder(string tag) => new TagElementBuilder(tag);

    public static readonly IElementBuilder Div = CreateBuilder("div");
    public static readonly IElementBuilder H1 = CreateBuilder("h1");
    public static readonly IElementBuilder P = CreateBuilder("p");
    public static readonly IElementBuilder A = CreateBuilder("a");
    public static readonly IElementBuilder Span = CreateBuilder("span");
    public static readonly IElementBuilder Nav = CreateBuilder("nav");
    public static readonly IElementBuilder Button = CreateBuilder("button");
    public static readonly IElementBuilder Em = CreateBuilder("em");
    public static readonly IElementBuilder Table = CreateBuilder("table");
    public static readonly IElementBuilder Thead = CreateBuilder("thead");
    public static readonly IElementBuilder Tr = CreateBuilder("tr");
    public static readonly IElementBuilder Th = CreateBuilder("th");
    public static readonly IElementBuilder Td = CreateBuilder("td");
    public static readonly IElementBuilder Tbody = CreateBuilder("tbody");
}
