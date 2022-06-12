using System.Linq;
using SharpCompose.Base;

namespace TestSharpCompose.ComposeTester.Matchers;

public class HasTextNodeMatcher : INodeMatcher
{
    private readonly string? textToMatch;

    public HasTextNodeMatcher(string? textToMatch)
    {
        this.textToMatch = textToMatch;
    }

    public bool Match(Composer.Scope scope)
    {
        return scope.Modifier
            .SqueezeModifiers()
            .Any(m => m is TextDrawModifier textDrawModifier &&
                      (textToMatch == null || textToMatch == textDrawModifier.Text));
    }
}