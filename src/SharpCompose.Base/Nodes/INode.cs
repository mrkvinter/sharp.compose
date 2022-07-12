namespace SharpCompose.Base.Nodes;

public interface INode
{
    Remembered Remembered { get; }

    INode? Parent { get; }

    void Clear();
}