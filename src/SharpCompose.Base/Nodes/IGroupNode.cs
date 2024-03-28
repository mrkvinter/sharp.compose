namespace SharpCompose.Base.Nodes;

public interface IGroupNode : INode
{
    IEnumerable<LayoutNode> LayoutNodes { get; }
    IEnumerable<IGroupNode> GroupNodes { get; }
    IReadOnlyList<INode> Children { get; }

    Dictionary<int, object> Locals { get; set; }

    List<INode> UnusedChildren { get; }

    HashSet<string> UnusedRememberedKeys { get; }

    Dictionary<string, int> CountNodes { get; }

    Action? Content { get; set; }
    long Id { get; }
    bool Changed { get; set; }
    bool HasExternalState { get; }

    void SaveUnused();

    void AddChild(INode node);
    void RemoveChild(INode node);
}