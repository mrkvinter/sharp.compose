using System;
using System.Collections.Generic;
using FakeItEasy;
using SharpCompose.Drawer.Core;
using SharpCompose.WebTags;
using SharpCompose.WebTags.ElementBuilder;

namespace TestSharpCompose.TestComposer;

public class TestTreeBuilder : TreeBuilder
{
    private static Stack<HtmlNode> nodes = new();

    public HtmlNode Root { get; }

    public TestTreeBuilder() : base(A.Fake<ICanvas>())
    {
        Root = new HtmlNode();

        nodes.Push(Root);
    }

    public override void StartNode(IElementBuilder elementBuilder, IReadOnlyDictionary<string, object> attrs)
    {
        var node = new HtmlNode();
        nodes.Peek().Child.Add(node);

        nodes.Push(node);
        if (elementBuilder is TextElementBuilder textElementBuilder)
        {
            node.Content = textElementBuilder.Text;
        }
        else if (elementBuilder is TagElementBuilder)
        {
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

    public string? Content;

    public string? Id;

    public Action? OnClick;
}