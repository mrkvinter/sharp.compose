using System;
using System.Collections.Generic;
using System.Linq;
using SharpCompose.Base.Modifiers;
using SharpCompose.Base.Nodes;
using SharpCompose.Drawer.Core;
using TestSharpCompose.ComposeTester.Matchers;

namespace TestSharpCompose.ComposeTester;

public static class NodeExtensions
{
    public static Node? OnNode(this Node node, INodeMatcher nodeMatcher)
    {
        var scopes = new Queue<LayoutNode>();
        scopes.Enqueue(node.LayoutNode);
        while (scopes.TryDequeue(out var layoutNode))
        {
            if (nodeMatcher.Match(layoutNode))
                return node with { LayoutNode = layoutNode };

            layoutNode.GroupNode.LayoutNodes.ForEach(scopes.Enqueue);
        }

        return default;
    }

    public static Node? OnNodeWithId(this Node node, string id)
        => node.OnNode(new IdNodeMatcher(id));

    public static Node? OnNodeWithText(this Node node, string text)
        => node.OnNode(new HasTextNodeMatcher(text));

    public static Node? OnNodeWith(this Node node, Func<LayoutNode, bool> condition)
        => node.OnNode(new ConditionNodeMatcher(condition));

    public static Node? OnNodeWithModifier<T>(this Node node, Func<T, bool>? condition = null)
        where T : IModifier
        => node.OnNode(new ConditionNodeMatcher(e =>
        {
            var modifier = e.SqueezeModifiers.OfType<T>();
            return condition == null || modifier.Any(condition);
        }));

    public static Node PerformClick(this Node node)
    {
        var mousePos =
            node.LayoutNode.Modifier.SqueezeModifiers()
                .Select(e => e as OnMouseInputModifier)
                .Where(e => e != null)
                .Cast<OnMouseInputModifier>()
                .First()
                .BoundState;
        
        node.ComposeTester.Click(mousePos);

        return node;
    }
}

public sealed class TestIdModifier : IModifier.IElement
{
    public string Id { get; }
    
    public TestIdModifier(string id)
    {
        Id = id;
    }
}

public static class TestModifierExtensions
{
    public static T Id<T>(this T self, string id) where T : IScopeModifier<T>
        => self
            .Then(new TestIdModifier(id))
            .Then(new DebugModifier {ScopeName = id});
}