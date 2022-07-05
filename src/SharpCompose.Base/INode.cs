namespace SharpCompose.Base;

public interface INode
{
    IEnumerable<LayoutNode> Nodes { get; }

    List<INode> UnusedChildren { get; }

    Remembered Remembered { get; }

    Dictionary<int, object> Locals { get; }

    INode? Parent { get; }

    void Clear();
        
    void SaveUnused();
        
    void AddChild(INode scope);
}