﻿using Microsoft.AspNetCore.Components;

namespace SharpCompose.Base;

public class ATagAttributesBuilder : BaseTagAttributesBuilder<ATagAttributesBuilder>
{
    public ATagAttributesBuilder Href(string href) => Attr("href", href);
}

public class CommonTagAttributesBuilder : BaseTagAttributesBuilder<CommonTagAttributesBuilder>
{
}

public class BaseTagAttributesBuilder<T> : BaseAttributesBuilder
    where T : BaseTagAttributesBuilder<T>
{
    protected T Attr(string name, object value)
    {
        attributes[name] = value;

        return (T) this;
    }

    public T Id(string id) => Attr("id", id);

    public T Class(params string[] classes) => Attr("class", string.Join(" ", classes));

    //todo: make styleBuilder here
    public T Style(string style) => Attr("style", style);

    public T OnClick(Action onClick) => Attr("onclick", onClick);

    public T OnChange(Action<ChangeEventArgs> onChange) => Attr("onchange", onChange);

    public T OnInput(Action<ChangeEventArgs> onInput) => Attr("oninput", onInput);

    public T AreaHidden(bool areaHidden) => Attr("area-hidden", areaHidden);
}