using SharpCompose.Base.Nodes;

namespace TestSharpCompose.ComposeTester.Matchers;

public interface INodeMatcher
{
    bool Match(LayoutNode layoutNode);
}