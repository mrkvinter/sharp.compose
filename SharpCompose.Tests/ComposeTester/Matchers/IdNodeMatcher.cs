using System.Linq;
using SharpCompose.Base;

namespace TestSharpCompose.ComposeTester.Matchers;

public class IdNodeMatcher : INodeMatcher
{
    private readonly string id;

    public IdNodeMatcher(string id)
    {
        this.id = id;
    }

    public bool Match(Composer.Scope scope)
    {
        return scope.Modifier
            .SqueezeModifiers()
            .Any(m => m is TestIdModifier idModifier && idModifier.Id == id);
    }
}