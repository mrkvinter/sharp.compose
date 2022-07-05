namespace SharpCompose.Base;

public class GroupNode : INode
{
    private readonly List<INode> children = new();

    public List<INode> UnusedChildren { get; } = new();

    public Remembered Remembered { get; } = new();

    public Dictionary<int, object> Locals { get; } = new();

    public INode Parent { get; init; }

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

    public void AddChild(INode scope)
    {
        children.Add(scope);
    }
        
    public void Clear()
    {
        children.ForEach(c => c.Clear());
        children.Clear();
        Remembered.Clear();
    }
}