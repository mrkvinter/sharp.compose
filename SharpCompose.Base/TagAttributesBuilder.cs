namespace SharpCompose.Base;

public abstract class BaseAttributesBuilder
{
    public abstract void Build(Composer composer);
}

public class TagAttributesBuilder : BaseAttributesBuilder
{
    private string? id { get; set; }
    private string[]? classes { get; set; }
    private string? href { get; set; }
    private string? style { get; set; }
    private Action? onClick { get; set; }
    private bool? areaHidden { get; set; }

    public void Id(string id)
    {
        this.id = id;
    }

    public void Class(params string[] classes)
    {
        this.classes = classes;
    }

    public void Href(string href)
    {
        this.href = href;
    }

    //todo: make styleBuilder here
    public void Style(string style)
    {
        this.style = style;
    }

    public void OnClick(Action onClickAction)
    {
        this.onClick = onClickAction;
    }

    public void AreaHidden(bool areaHidden)
    {
        this.areaHidden = areaHidden;
    }

    public override void Build(Composer composer)
    {
        if (id != null)
            composer.AddAttribute("id", id);
        
        if (classes?.Length > 0)
            composer.AddAttribute("class", string.Join(" ", classes));

        if (href != null)
            composer.AddAttribute("href", href);

        if (style != null)
            composer.AddAttribute("style", style);

        if (onClick != null)
            composer.AddAttribute("onclick", onClick);

        if (areaHidden != null)
            composer.AddAttribute("area-hidden", areaHidden);
    }
}