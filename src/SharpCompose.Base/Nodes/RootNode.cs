using SharpCompose.Base.Modifiers;
using SharpCompose.Drawer.Core;

namespace SharpCompose.Base.Nodes;

public class RootNode : LayoutNode, IGroupNode
{
    private readonly GroupNode rootGroupNode;

    public RootNode(IGraphics graphics) : base(IModifier.Empty, graphics)
    {
        rootGroupNode = new GroupNode();
        GroupNode = rootGroupNode;
    }

    public Remembered Remembered { get; } = new();

    public IEnumerable<LayoutNode> Nodes => rootGroupNode.Nodes;

    public Dictionary<int, object> Locals => rootGroupNode.Locals;

    public List<INode> UnusedChildren => rootGroupNode.UnusedChildren;

    public Dictionary<string, int> CountNodes => rootGroupNode.CountNodes;

    public void SaveUnused()
    {
        rootGroupNode.SaveUnused();
    }

    public void AddChild(INode node)
    {
        rootGroupNode.AddChild(node);
    }
}