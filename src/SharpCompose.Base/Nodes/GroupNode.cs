namespace SharpCompose.Base.Nodes;

public class GroupNode : IGroupNode
{
    private readonly List<INode> children = new();

    public List<INode> UnusedChildren { get; } = new();
    public HashSet<string> UnusedRememberedKeys { get; } = new();
    public Remembered Remembered { get; } = new();
    public Dictionary<int, object> Locals { get; set; } = new();
    public INode? Parent { get; init; }
    public Dictionary<string, int> CountNodes { get; } = new();
    public Action? Content { get; set; }
    public long Id { get; init; }
    public bool Changed { get; set; }
    public bool HasExternalState { get; init; }
    public IReadOnlyList<INode> Children => children;

    public IEnumerable<LayoutNode> LayoutNodes
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
                        foreach (var scopeChild in groupNode.LayoutNodes)
                            yield return scopeChild;
                        break;
                    }
                }
            }
        }
    }

    public IEnumerable<IGroupNode> GroupNodes
    {
        get
        {
            foreach (var child in children)
            {
                if (child is IGroupNode groupNode)
                    yield return groupNode;

                if (child is LayoutNode layoutNode)
                    yield return layoutNode.GroupNode;
            }
        }
    }

    public void SaveUnused()
    {
        UnusedChildren.Clear();
        UnusedChildren.AddRange(children);

        UnusedRememberedKeys.Clear();
        UnusedRememberedKeys.UnionWith(Remembered.Keys);
    }

    public void AddChild(INode node)
    {
        children.Add(node);
    }

    public void RemoveChild(INode node)
    {
        children.Remove(node);
    }

    public void Clear()
    {
        children.ForEach(c => c.Clear());
        children.Clear();
        Remembered.Clear();
    }
}