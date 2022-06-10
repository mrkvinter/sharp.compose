using System;
using System.Collections.Generic;
using SharpCompose.Base;
using SharpCompose.Base.ElementBuilder;
using SharpCompose.WebTags;

namespace TestSharpCompose.TestComposer;

public class TestTreeBuilder : TreeBuilder
{
    private static Stack<HtmlNode> nodes = new();

    public HtmlNode Root { get; private set; }

    public static HtmlNode Current => nodes.Peek();

    public TestTreeBuilder() : base(new TestCanvas())
    {
    }

    public override void StartNode(IElementBuilder elementBuilder, IReadOnlyDictionary<string, object> attrs)
    {
        var node = new HtmlNode();
        if (nodes.Count > 0)
            nodes.Peek().Child.Add(node);
        else
            Root = node;

        nodes.Push(node);
        if (elementBuilder is TextElementBuilder textElementBuilder)
        {
            node.Content = textElementBuilder.Text;
        }
        else if (elementBuilder is TagElementBuilder tagElementBuilder)
        {
            node.Tag = tagElementBuilder.Tag;
            BuildAttributes(attrs, node);
        }
    }

    public override void EndNode()
    {
        nodes.Pop();
    }

    public void AddAttribute(string name, object? value, HtmlNode node)
    {
        switch (name.ToLower())
        {
            case "id":
                node.Id = (string) value!;
                break;
            case "class":
            {
                var classes = ((string) value!).Split(" ");
                node.Classes = classes;
                break;
            }
            case "onclick":
                node.OnClick = (Action) value!;
                break;
            default: throw new NotImplementedException($"Unknowing attribute name {name}");
        }
    }

    private void BuildAttributes(IReadOnlyDictionary<string, object> attributes, HtmlNode node)
    {
        foreach (var (attributeName, value) in attributes)
        {
            AddAttribute(attributeName, value, node);
        }
    }
}

public class HtmlNode
{
    public List<HtmlNode> Child { get; } = new();

    public string? Tag;

    public string? Content;

    public string? Id;

    public string[]? Classes;

    public Action? OnClick;
}