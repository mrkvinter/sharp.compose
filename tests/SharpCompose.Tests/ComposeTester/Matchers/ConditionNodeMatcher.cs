using System;
using SharpCompose.Base.Nodes;

namespace TestSharpCompose.ComposeTester.Matchers;

public class ConditionNodeMatcher : INodeMatcher
{
    private readonly Func<LayoutNode, bool> condition;
    
    public ConditionNodeMatcher(Func<LayoutNode, bool> condition)
    {
        this.condition = condition;
    }
    
    public bool Match(LayoutNode layoutNode)
    {
        return condition(layoutNode);
    }
}