using SharpCompose.Base;
using SharpCompose.Base.ElementBuilder;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Shapes;

namespace SharpCompose.WebTags;

public class FakeCanvas : ICanvas
{
    public void Draw()
    {
    }

    public (int w, int h) MeasureText(string text, double emSize, Font font) => (0, 0);

    public IGraphics StartGraphics() => new FakeGraphics();

    public void DrawGraphics(int x, int y, IGraphics graphics)
    {
    }

    public void Clear()
    {
    }

    public (int w, int h) Size { get; set; }

    public class FakeGraphics : IGraphics
    {
        public void FillRectangle((int x, int y) point, (int w, int h) size, Brush brush)
        {
        }

        public void StrokeShape((int x, int y) point, IShape shape, int width, int height, int lineWidth, Brush brush)
        {
        }

        public void DrawGraphics(IGraphics otherGraphics, int x, int y)
        {
        }

        public void DrawText(string text, double emSize, Font font, Brush brush, int x, int y)
        {
        }

        public void DrawShadow((int x, int y) point, (int w, int h) size, (int x, int y) offset, int blurRadius,
            IShape shape,
            Brush brush)
        {
        }

        public void Clip(IShape shape, (int x, int y) offset, (int w, int h) size)
        {
        }

        public void Clear()
        {
        }
    }
}

public class TreeBuilder
{
    public static TreeBuilder Instance { get; set; }

    private static int sequence;
    private static Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder = null!;

    protected TreeBuilder(ICanvas canvas)
    {
        Composer.Instance.Init(canvas);
    }

    public static void Build(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        TreeBuilder.builder = builder;
        sequence = 0;
    }

    public virtual void StartNode(IElementBuilder elementBuilder, IReadOnlyDictionary<string, object> attrs)
    {
        if (elementBuilder is TextElementBuilder textElementBuilder)
        {
            builder.AddContent(sequence++, textElementBuilder.Text);
        }
        else if (elementBuilder is TagElementBuilder tagElementBuilder)
        {
            builder.OpenElement(sequence++, tagElementBuilder.Tag);
            AddAttribute(attrs);
        }
    }

    public virtual void EndNode()
    {
        builder.CloseElement();
    }

    private void AddAttribute(string name, object? value)
    {
        if (value is string str)
            builder.AddAttribute(sequence++, name, str);
        else if (value is MulticastDelegate @delegate)
            builder.AddAttribute(sequence++, name, @delegate);
        else if (value is bool boolValue)
            builder.AddAttribute(sequence++, name, boolValue);
        else if (value == null)
            throw new NullReferenceException("Value attribute should be initialized");
        else
            throw new NotImplementedException($"Unknown type value: {value.GetType()}");
    }

    private void AddAttribute(IReadOnlyDictionary<string, object> attrs)
    {
        foreach (var (attributeName, value) in attrs)
        {
            AddAttribute(attributeName, value);
        }
    }
}