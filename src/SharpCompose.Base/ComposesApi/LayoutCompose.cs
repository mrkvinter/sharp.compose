using System.Diagnostics.CodeAnalysis;
using SharpCompose.Base.Layouting;
using SharpCompose.Base.Modifiers;

namespace SharpCompose.Base.ComposesApi;

public static partial class BaseCompose
{
    public static void Layout(IModifier modifier, Action? content, Measure measure)
    {
        Composer.Instance.StartScope(modifier, measure);
        content?.Invoke();
        Composer.Instance.StopScope();
    }

    public static void Box(ScopeModifier? boxModifier = default, IAlignment? alignment = null,
        Action? content = null) =>
        Layout((boxModifier?.SelfModifier ?? IModifier.Empty).Then(new DebugModifier {ScopeName = nameof(Box)}),
            content,
            BoxLayout.Measure(alignment ?? Alignment.TopStart));

    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public static void BoxWithConstraints(ScopeModifier? boxModifier = default, IAlignment? alignment = null,
        Action<Constraints>? content = null)
    {
        LayoutNode layoutNode = default!;
        Composer.Instance.StartScope(
            (boxModifier?.SelfModifier ?? IModifier.Empty).Then(new DebugModifier
                {ScopeName = nameof(BoxWithConstraints)}), (_, constraints) =>
            {
                Composer.Instance.Scopes.Push(layoutNode);
                content?.Invoke(constraints);
                Composer.Instance.StopScope();

                var measures = layoutNode.Children.SelectMany(e => e.Nodes.Select(e => e.Measurable)).ToArray();
                return BoxLayout.Measure(alignment ?? Alignment.TopStart)(measures, constraints);
            });

        layoutNode = (LayoutNode)Composer.Instance.Scopes.Pop();
    }

    public static void Column(ScopeModifier? columnModifier = default, IAlignment? horizontalAlignment = null,
        Action? content = null) =>
        Layout((columnModifier?.SelfModifier ?? IModifier.Empty).Then(new DebugModifier {ScopeName = nameof(Column)}),
            content,
            RowColumnLayout.MeasureColumn(horizontalAlignment ?? Alignment.TopStart));

    public static void Row(ScopeModifier? rowModifier = default, IAlignment? verticalAlignment = null,
        Action? content = null) =>
        Layout((rowModifier?.SelfModifier ?? IModifier.Empty).Then(new DebugModifier {ScopeName = nameof(Row)}),
            content,
            RowColumnLayout.MeasureRow(verticalAlignment ?? Alignment.TopStart));

    public static void Spacer(ScopeModifier? spacerModifier = default) =>
        Layout((spacerModifier?.SelfModifier ?? IModifier.Empty).Then(new DebugModifier {ScopeName = nameof(Spacer)}),
            null, EmptyMeasure);

    private static readonly Measure EmptyMeasure = (_, constraints) => new MeasureResult
        {Width = constraints.MinWidth, Height = constraints.MinHeight, Placeable = EmptyPlaceable};

    private static readonly Action<int, int> EmptyPlaceable = (_, _) => { };
}