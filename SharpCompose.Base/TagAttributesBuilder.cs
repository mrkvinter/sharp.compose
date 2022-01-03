using Microsoft.AspNetCore.Components;

namespace SharpCompose.Base;

public abstract class BaseAttributesBuilder
{
    protected readonly Dictionary<string, object> attributes = new();

    public IReadOnlyDictionary<string, object> Attributes => attributes;
}

public class TagAttributesBuilder : BaseAttributesBuilder
{
    public void Id(string id)
    {
        attributes["id"] = id;
    }

    public void Class(params string[] classes)
    {
        attributes["class"] = string.Join(" ", classes);
    }

    public void Href(string href)
    {
        attributes["href"] = href;
    }

    //todo: make styleBuilder here
    public void Style(string style)
    {
        attributes["style"] = style;
    }

    public void OnClick(Action onClickAction)
    {
        attributes["onclick"] = onClickAction;
    }

    public void OnChange(Action<ChangeEventArgs> onChangeAction)
    {
        attributes["onchange"] = onChangeAction;
    }

    public void OnInput(Action<ChangeEventArgs> onInputAction)
    {
        attributes["oninput"] = onInputAction;
    }

    public void AreaHidden(bool areaHidden)
    {
        attributes["area-hidden"] = areaHidden;
    }
}