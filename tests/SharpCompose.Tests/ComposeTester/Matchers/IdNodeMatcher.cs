using System.Linq;
using SharpCompose.Base.Nodes;

namespace TestSharpCompose.ComposeTester.Matchers;

public class IdNodeMatcher : INodeMatcher
{
    private readonly string id;

    public IdNodeMatcher(string id)
    {
        this.id = id;
    }

    public bool Match(LayoutNode layoutNode)
    {
        return layoutNode.Modifier
            .SqueezeModifiers()
            .Any(m => m is TestIdModifier idModifier && idModifier.Id == id);
    }
}