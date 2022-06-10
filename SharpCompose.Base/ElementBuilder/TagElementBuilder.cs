using SharpCompose.Drawer.Core;

namespace SharpCompose.Base.ElementBuilder;

[Obsolete("Used to for Blazor integrating")]
public class TagElementBuilder : IElementBuilder
{
    public string Tag { get; }

    private TagElementBuilder(string tag)
    {
        Tag = tag;
    }

    public override string ToString()
    {
        return $"{nameof(TagElementBuilder)} <{Tag}>";
    }

    private static IElementBuilder CreateBuilder(string tag) => new TagElementBuilder(tag);

    public static readonly IElementBuilder Div = CreateBuilder("div");
    public static readonly IElementBuilder H1 = CreateBuilder("h1");
    public static readonly IElementBuilder H2 = CreateBuilder("h2");
    public static readonly IElementBuilder H3 = CreateBuilder("h3");
    public static readonly IElementBuilder P = CreateBuilder("p");
    public static readonly IElementBuilder A = CreateBuilder("a");
    public static readonly IElementBuilder Span = CreateBuilder("span");
    public static readonly IElementBuilder Nav = CreateBuilder("nav");
    public static readonly IElementBuilder Button = CreateBuilder("button");
    public static readonly IElementBuilder Input = CreateBuilder("input");
    public static readonly IElementBuilder Em = CreateBuilder("em");
    public static readonly IElementBuilder Table = CreateBuilder("table");
    public static readonly IElementBuilder Thead = CreateBuilder("thead");
    public static readonly IElementBuilder Tr = CreateBuilder("tr");
    public static readonly IElementBuilder Th = CreateBuilder("th");
    public static readonly IElementBuilder Td = CreateBuilder("td");
    public static readonly IElementBuilder Tbody = CreateBuilder("tbody");
    public static readonly IElementBuilder Ul = CreateBuilder("ul");
    public static readonly IElementBuilder Li = CreateBuilder("li");
    public static readonly IElementBuilder Article = CreateBuilder("article");
    public static readonly IElementBuilder Main = CreateBuilder("main");
    public static readonly IElementBuilder I = CreateBuilder("i");
    public static readonly IElementBuilder Strong = CreateBuilder("strong");

    public (int w, int h) CalculateVisualSize(Composer.Scope scope)
    {
        throw new NotImplementedException();
    }

    public (int w, int h) CalculateRealSize(Composer.Scope scope)
    {
        throw new NotImplementedException();
    }

    public void Draw(Composer.Scope scope, ICanvas canvas, int pointerX, int pointerY)
    {
        throw new NotImplementedException();
    }
}