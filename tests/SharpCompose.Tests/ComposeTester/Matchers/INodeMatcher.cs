using SharpCompose.Base;

namespace TestSharpCompose.ComposeTester.Matchers;

public interface INodeMatcher
{
    bool Match(Composer.Scope scope);
}