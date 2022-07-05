using System.Linq;
using SharpCompose.Base;
using SharpCompose.Base.Modifiers;

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