namespace SharpCompose.Base.Nodes;

public interface IUINode : INode
{
    IGroupNode GroupNode { get; set; }
}