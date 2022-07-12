namespace SharpCompose.Base.Nodes;

public class GroupNode : IGroupNode
{
    private readonly List<INode> children = new();

    public List<INode> UnusedChildren { get; } = new();

    public Remembered Remembered { get; } = new();

    public Dictionary<int, object> Locals { get; } = new();

    public INode Parent { get; init; }

    public Dictionary<string, int> CountNodes { get; } = new();

    public IEnumerable<LayoutNode> Nodes
    {
        get
        {
            foreach (var child in children)
            {
                switch (child)
                {
                    case LayoutNode scope:
                        yield return scope;
                        break;
                    case GroupNode groupNode:
                    {
                        foreach (var scopeChild in groupNode.Nodes)
                            yield return scopeChild;
                        break;
                    }
                }
            }
        }
    }

    public void SaveUnused()
    {
        UnusedChildren.Clear();
        UnusedChildren.AddRange(children);
    }

    public void AddChild(INode node)
    {
        children.Add(node);
    }
        
    public void Clear()
    {
        children.ForEach(c => c.Clear());
        children.Clear();
        Remembered.Clear();
    }
}