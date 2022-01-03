using Microsoft.AspNetCore.Components.Rendering;
using SharpCompose.Base.ElementBuilder;

namespace SharpCompose.Base.Composers;

public class RenderTreeComposer : Composer
{
    private int sequence;
    private Action<IReadOnlyDictionary<string, object>>? attributeBuilder;

    public void Build(RenderTreeBuilder builder)
    {
        if (Root == null)
            throw new NullReferenceException("Root element wasn't initialized. Call");

        sequence = 0;

        void AddAttr(IReadOnlyDictionary<string, object> attr) => AddAttribute(attr, builder);

        attributeBuilder += AddAttr;

        foreach (var scope in Root.Child)
        {
            BuildNode(scope, builder);
        }

        attributeBuilder -= AddAttr;
    }

    private void BuildNode(Scope scope, RenderTreeBuilder builder)
    {
        StartNode(scope, builder);
        foreach (var childScope in scope.Child)
        {
            BuildNode(childScope, builder);
        }

        EndNode(scope, builder);
    }

    private void StartNode(Scope scope, RenderTreeBuilder builder)
    {
        if (scope.ElementBuilder is TextElementBuilder textElementBuilder)
        {
            builder.AddContent(sequence++, textElementBuilder.Text);
        }
        else if (scope.ElementBuilder is TagElementBuilder tagElementBuilder)
        {
            builder.OpenElement(sequence++, tagElementBuilder.Tag);
            scope.AttributeBuilder(this);
        }
    }

    private void EndNode(Scope scope, RenderTreeBuilder builder)
    {
        if (scope.ElementBuilder is TagElementBuilder)
        {
            builder.CloseElement();
        }
        else if (scope.ElementBuilder == null)
        {
            builder.CloseElement();
        }
    }

    public void AddAttribute(string name, object? value, RenderTreeBuilder builder)
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

    private void AddAttribute(IReadOnlyDictionary<string, object> attrs, RenderTreeBuilder builder)
    {
        foreach (var (attributeName, value) in attrs)
        {
            AddAttribute(attributeName, value, builder);
        }
    }

    public override void BuildAttributes(IReadOnlyDictionary<string, object> attributes)
    {
        attributeBuilder?.Invoke(attributes);

    }
}