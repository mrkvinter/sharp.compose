using System.Linq;
using SharpCompose.Base.Modifiers;
using SharpCompose.Base.Nodes;

namespace TestSharpCompose.ComposeTester.Matchers;

public class HasTextNodeMatcher : INodeMatcher
{
    private readonly string? textToMatch;

    public HasTextNodeMatcher(string? textToMatch)
    {
        this.textToMatch = textToMatch;
    }

    public bool Match(LayoutNode layoutNode)
    {
        return layoutNode.Modifier
            .SqueezeModifiers()
            .Any(m => m is TextDrawModifier textDrawModifier &&
                      (textToMatch == null || textToMatch == textDrawModifier.Text));
    }
}