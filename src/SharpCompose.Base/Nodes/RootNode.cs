using SharpCompose.Base.Modifiers;
using SharpCompose.Drawer.Core;

namespace SharpCompose.Base.Nodes;

public class RootNode : LayoutNode, IGroupNode
{
    private readonly GroupNode rootGroupNode;

    public RootNode(IGraphics graphics, long id) : base(IModifier.Empty, graphics)
    {
        rootGroupNode = new GroupNode
        {
            Id = id
        };
        GroupNode = rootGroupNode;
    }

    public IEnumerable<LayoutNode> LayoutNodes => rootGroupNode.LayoutNodes;
    public IEnumerable<IGroupNode> GroupNodes => rootGroupNode.GroupNodes;
    public IReadOnlyList<INode> Children => rootGroupNode.Children;

    public Dictionary<int, object> Locals
    {
        get => rootGroupNode.Locals;
        set => rootGroupNode.Locals = value;
    }

    public List<INode> UnusedChildren => rootGroupNode.UnusedChildren;

    public HashSet<string> UnusedRememberedKeys => rootGroupNode.UnusedRememberedKeys;

    public Dictionary<string, int> CountNodes => rootGroupNode.CountNodes;
    public Action? Content { get => rootGroupNode.Content; set => rootGroupNode.Content = value; }
    public long Id => rootGroupNode.Id;
    public bool Changed { get => rootGroupNode.Changed; set => rootGroupNode.Changed = value; }
    public bool HasExternalState => rootGroupNode.HasExternalState;

    public void SaveUnused()
    {
        rootGroupNode.SaveUnused();
    }

    public void AddChild(INode node)
    {
        rootGroupNode.AddChild(node);
    }

    public void RemoveChild(INode node)
    {
        rootGroupNode.RemoveChild(node);
    }
}