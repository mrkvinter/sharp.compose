using System.Collections.Generic;
using System.Linq;
using SharpCompose.Base;
using SharpCompose.Base.Modifiers;
using SharpCompose.Drawer.Core;
using TestSharpCompose.ComposeTester.Matchers;

namespace TestSharpCompose.ComposeTester;

public static class NodeExtensions
{
    public static Node? OnNode(this Node node, INodeMatcher nodeMatcher)
    {
        var scopes = new Queue<LayoutNode>();
        scopes.Enqueue(node.LayoutNode);
        while (scopes.TryDequeue(out var scope))
        {
            if (nodeMatcher.Match(scope))
                return new Node(scope);

            foreach (var child in scope.Children)
                child.Nodes.ForEach(e => scopes.Enqueue(e));
        }

        return default;
    }

    public static Node? OnNodeWithId(this Node node, string id)
        => node.OnNode(new IdNodeMatcher(id));

    public static Node? OnNodeWithText(this Node node, string text)
        => node.OnNode(new HasTextNodeMatcher(text));

    public static Node? OnNodeWithText(this Node node)
        => node.OnNode(new HasTextNodeMatcher(null));

    public static Node PerformClick(this Node node)
    {
        var inputModifier =
            node.LayoutNode.Modifier.SqueezeModifiers()
                .Select(e => e as OnMouseInputModifier)
                .Where(e => e != null)
                .Cast<OnMouseInputModifier>()
                .ToArray();

        inputModifier.ForEach(e => e.OnMouseOver?.Invoke());
        inputModifier.ForEach(e => e.OnMouseDown?.Invoke());
        inputModifier.ForEach(e => e.OnMouseUp?.Invoke());

        return node;
    }
}

public sealed class TestIdModifier : IModifier.IElement
{
    public string Id { get; init; }
}

public static class TestModifierExtensions
{
    public static T Id<T>(this T self, string id) where T : IScopeModifier<T>
        => self.Then(new TestIdModifier {Id = id});
}