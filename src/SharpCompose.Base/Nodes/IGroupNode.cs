namespace SharpCompose.Base.Nodes;

public interface IGroupNode : INode
{
    IEnumerable<LayoutNode> Nodes { get; }

    Dictionary<int, object> Locals { get; }

    List<INode> UnusedChildren { get; }

    Dictionary<string, int> CountNodes { get; }

    void SaveUnused();

    void AddChild(INode node);
}