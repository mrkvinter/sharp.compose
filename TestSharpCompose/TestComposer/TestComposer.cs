using System;
using System.Collections.Generic;
using SharpCompose.Base;
using SharpCompose.Base.ElementBuilder;

namespace TestSharpCompose.TestComposer;

public class TestComposer : Composer
{
    private HtmlNode? current;
    private Action<IReadOnlyDictionary<string, object>>? attributeBuilder;

    public HtmlNode Build()
    {
        void AddAttr(IReadOnlyDictionary<string, object> attrs) => BuildAttributes(attrs, current!);

        attributeBuilder += AddAttr;

        var root = new HtmlNode();

        foreach (var scope in Root!.Child)
        {
            BuildNode(scope, root);
        }

        return root;
    }

    private void BuildNode(Scope scope, HtmlNode node)
    {
        StartNode(scope, node);
        foreach (var childScope in scope.Child)
        {
            var childNode = new HtmlNode();
            node.Child.Add(childNode);
            BuildNode(childScope, childNode);
        }

        EndNode(scope, node);
    }


    private void StartNode(Scope scope, HtmlNode node)
    {
        if (scope.ElementBuilder is TextElementBuilder textElementBuilder)
        {
            node.Content = textElementBuilder.Text;
        }
        else if (scope.ElementBuilder is TagElementBuilder tagElementBuilder)
        {
            node.Tag = tagElementBuilder.Tag;
            current = node;
            scope.AttributeBuilder(this);
        }
    }

    private void EndNode(Scope scope, HtmlNode node)
    {
    }

    public void AddAttribute(string name, object? value, HtmlNode node)
    {
        switch (name.ToLower())
        {
            case "id": node.Id = (string) value!; break;
            case "class":
            {
                var classes = ((string) value!).Split(" ");
                node.Classes = classes;
                break;
            }
            case "onclick": node.OnClick = (Action) value!; break;
            default: throw new NotImplementedException($"Unknowing attribute name {name}");
        }
    }


    public override void BuildAttributes(IReadOnlyDictionary<string, object> attributes)
    {
        attributeBuilder?.Invoke(attributes);
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