using System.Diagnostics.CodeAnalysis;
using SharpCompose.Base.Layouting;
using SharpCompose.Base.Modifiers;
using SharpCompose.Base.Modifiers.Extensions;
using SharpCompose.Base.Nodes;

namespace SharpCompose.Base.ComposesApi;

public static partial class BaseCompose
{
    [UiComposable]
    public static void Layout(IModifier modifier, Action? content, Measure measure)
    {
        Composer.Instance.StartNode(modifier, measure);
        Composer.Instance.StartGroup(content);
        Composer.Instance.EndGroup();
        Composer.Instance.EndNode();
    }

    [UiComposable]
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
        Composer.Instance.StartNode(
            (boxModifier?.SelfModifier ?? IModifier.Empty).Then(new DebugModifier
                {ScopeName = nameof(BoxWithConstraints)}), [RootComposableApi] (_, constraints) =>
            {
                Composer.Instance.Groups.Push(layoutNode.GroupNode);
                layoutNode.GroupNode.Changed = true;
                content?.Invoke(constraints);
                Composer.Instance.EndGroup();

                var measures = layoutNode.GroupNode.LayoutNodes.Select(e => e.Measurable).ToArray();
                return BoxLayout.Measure(alignment ?? Alignment.TopStart)(measures, constraints);
            });
        Composer.Instance.StartGroup(null);
        Composer.Instance.Groups.Pop();

        layoutNode = (LayoutNode)Composer.Instance.UINodes.Pop();
    }
    
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public static void AbsoluteScope(ScopeModifier? boxModifier = default,
        Action? content = null)
    {
        LayoutNode absoluteNode = default!;
        Composer.Instance.StartNode(
            (boxModifier?.SelfModifier ?? IModifier.Empty).Then(new DebugModifier
                {ScopeName = "UnderAbsolute"}), [RootComposableApi] (measures, constraints) =>
            {
                var measure = BoxLayout.Measure(Alignment.TopStart)(measures, constraints);
                
                Composer.Instance.Groups.Push(absoluteNode.GroupNode);
                absoluteNode.GroupNode.Changed = true;
                content?.Invoke();
                Composer.Instance.EndGroup();
                
                var absoluteMeasures = BoxLayout.Measure(Alignment.TopStart)(absoluteNode.GroupNode.LayoutNodes.Select(e => e.Measurable).ToArray(), constraints);

                return measure with
                {
                    Placeable = (x, y) =>
                    {
                        measure.Placeable(x, y);
                        absoluteMeasures.Placeable(x, y);
                    }
                };
            });

        Composer.Instance.StartGroup(null);
        Composer.Instance.EndGroup();
        _ = (LayoutNode)Composer.Instance.UINodes.Pop();

        Composer.Instance.StartNode(Modifier.FillMaxWidth().FillMaxHeight().Then(new DebugModifier { ScopeName = "Absolute" }).SelfModifier,
            (_, _) => new MeasureResult { Placeable = (_, _) => {}});
        Composer.Instance.StartGroup(null);
        Composer.Instance.Groups.Pop();

        absoluteNode = (LayoutNode)Composer.Instance.UINodes.Pop();
    }

    [UiComposable]
    public static void Column(ScopeModifier? columnModifier = default, IAlignment? horizontalAlignment = null,
        Action? content = null) =>
        Layout((columnModifier?.SelfModifier ?? IModifier.Empty).Then(new DebugModifier {ScopeName = nameof(Column)}),
            content,
            RowColumnLayout.MeasureColumn(horizontalAlignment ?? Alignment.TopStart));

    [UiComposable]
    public static void Row(ScopeModifier? rowModifier = default, IAlignment? verticalAlignment = null,
        Action? content = null) =>
        Layout((rowModifier?.SelfModifier ?? IModifier.Empty).Then(new DebugModifier {ScopeName = nameof(Row)}),
            content,
            RowColumnLayout.MeasureRow(verticalAlignment ?? Alignment.TopStart));

    [UiComposable]
    public static void Spacer(ScopeModifier? spacerModifier = default) =>
        Layout((spacerModifier?.SelfModifier ?? IModifier.Empty).Then(new DebugModifier {ScopeName = nameof(Spacer)}),
            null, EmptyMeasure);

    private static readonly Measure EmptyMeasure = (_, constraints) => new MeasureResult
        {Width = constraints.MinWidth, Height = constraints.MinHeight, Placeable = EmptyPlaceable};

    private static readonly Action<int, int> EmptyPlaceable = (_, _) => { };
}