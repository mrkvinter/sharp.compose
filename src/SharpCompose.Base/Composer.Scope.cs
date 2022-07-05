using SharpCompose.Base.Layouting;
using SharpCompose.Base.Modifiers;
using SharpCompose.Base.Modifiers.DrawableModifiers;
using SharpCompose.Base.Modifiers.LayoutModifiers;
using SharpCompose.Drawer.Core;

namespace SharpCompose.Base;

public class LayoutNode : INode
{
    internal IModifier Modifier { get; private set; }

    public List<INode> UnusedChildren { get; } = new();

    public Dictionary<int, object> Locals { get; } = new();

    public INode? Parent { get; init; }

    internal Measure Measure { get; set; }

    public IEnumerable<LayoutNode> Nodes
    {
        get { yield return this; }
    }

    private readonly List<INode> children = new();
    private readonly IGraphics graphics;

    public LayoutNode(IModifier modifier, IGraphics graphics)
    {
        this.Modifier = modifier;
        this.graphics = graphics;
    }

    public IReadOnlyCollection<INode> Children => children;

    public void Update(IModifier modifier)
    {
        this.Modifier = modifier;
    }

    public Remembered Remembered { get; } = new();

    public string Name { get; internal set; } = "";

    public bool Changed { get; set; }

    public Measurable Measurable => GetMeasure();

    public void ClearGraphics()
    {
        graphics.Clear();
        children.SelectMany(e => e.Nodes).ForEach(e => e.ClearGraphics());
    }

    private Measurable GetMeasure()
    {
        var measurable = new Measurable
        {
            Measure = constraints =>
            {
                return Measure(children
                    .SelectMany(e => e.Nodes.Select(x => x.Measurable)).ToArray(), constraints);
            }
        };

        var modifiers = Modifier.SqueezeModifiers();

        return modifiers.Aggregate(measurable, (current, m) => m switch
        {
            ILayoutModifier layoutModifier => layoutModifier.Introduce(current),
            IDrawableLayerModifier drawableModifier => drawableModifier.Introduce(current, graphics),
            IParentDataModifier parentDataModifier => parentDataModifier.Introduce(current),
            _ => current
        });
    }

    public void DrawNode(ICanvas canvas)
    {
        canvas.DrawGraphics(0, 0, graphics);
        children.SelectMany(e => e.Nodes).ForEach(c => c.DrawNode(canvas));
    }

    public void SaveUnused()
    {
        UnusedChildren.Clear();
        UnusedChildren.AddRange(children);
    }

    public void Clear()
    {
        children.ForEach(c => c.Clear());
        children.Clear();
        Remembered.Clear();
    }

    public void AddChild(INode scope)
    {
        children.Add(scope);
    }

    public void RemoveChild(LayoutNode layoutNode)
    {
        children.Remove(layoutNode);
    }

    public override string ToString() =>
        $"{nameof(LayoutNode)} ({Name}) [{string.Join("->", Children.SelectMany(e => e.Nodes.Select(e => e.Name)))}]";
}