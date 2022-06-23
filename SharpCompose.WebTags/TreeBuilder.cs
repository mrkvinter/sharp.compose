using SharpCompose.Base;
using SharpCompose.Drawer.Core;
using SharpCompose.WebTags.ElementBuilder;

namespace SharpCompose.WebTags;

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