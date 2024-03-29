﻿namespace SharpCompose.WebTags;

public abstract class BaseAttributesBuilder
{
    protected readonly Dictionary<string, object> attributes = new();

    public IReadOnlyDictionary<string, object> Attributes => attributes;
}
