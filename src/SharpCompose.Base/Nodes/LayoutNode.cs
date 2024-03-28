using SharpCompose.Base.Layouting;
using SharpCompose.Base.Modifiers;
using SharpCompose.Base.Modifiers.DrawableModifiers;
using SharpCompose.Base.Modifiers.LayoutModifiers;
using SharpCompose.Drawer.Core;

namespace SharpCompose.Base.Nodes;

public class LayoutNode : IUINode
{
    internal IModifier Modifier { get; private set; }

    internal IModifier[] SqueezeModifiers => Modifier.SqueezeModifiers();

    public IGroupNode GroupNode { get; set; } = default!;

    public INode? Parent { get; init; }

    public Remembered Remembered { get; } = new();

    internal Measure Measure { get; set; }


    private readonly IGraphics graphics;

    public LayoutNode(IModifier modifier, IGraphics graphics)
    {
        Modifier = modifier;
        this.graphics = graphics;
    }

    public void Update(IModifier modifier)
    {
        Modifier = modifier;
    }

    public string Name { get; internal set; } = "";

    public Measurable Measurable => GetMeasure();

    public void ClearGraphics()
    {
        graphics.Clear();
        GroupNode.LayoutNodes.ForEach(e => e.ClearGraphics());
    }

    private Measurable GetMeasure()
    {
        var measurable = new Measurable
        {
            Measure = constraints =>
                Measure(GroupNode.LayoutNodes.Select(x => x.Measurable).ToArray(), constraints)
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
        GroupNode.LayoutNodes.ForEach(c => c.DrawNode(canvas));
    }

    public void Clear()
    {
        GroupNode.Clear();
        foreach (var node in GroupNode.LayoutNodes)
        {
            node.Clear();
        }
        Remembered.Clear();
    }

    public override string ToString() =>
        $"{nameof(LayoutNode)} ({Name}) [{string.Join("->", GroupNode.LayoutNodes.Select(e => e.Name))}]";
}