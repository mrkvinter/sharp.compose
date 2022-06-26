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

    public static void Box(ScopeModifier? boxModifier = default, IAlignment? alignment = null, Action? content = null) =>
        Layout((boxModifier?.SelfModifier ?? IModifier.Empty).Then(new DebugModifier {ScopeName = nameof(Box)}),
            content,
            BoxLayout.Measure(alignment ?? new BiasAlignment(-1, -1)));

    public static void Column(ScopeModifier? columnModifier = default, IAlignment? horizontalAlignment = null, Action? content = null) =>
        Layout((columnModifier?.SelfModifier ?? IModifier.Empty).Then(new DebugModifier {ScopeName = nameof(Column)}),
            content, 
            RowColumnLayout.MeasureColumn(horizontalAlignment ?? new BiasAlignment(-1, 0)));

    public static void Row(ScopeModifier? rowModifier = default, IAlignment? verticalAlignment = null, Action? content = null) =>
        Layout((rowModifier?.SelfModifier ?? IModifier.Empty).Then(new DebugModifier {ScopeName = nameof(Row)}),
            content,
            RowColumnLayout.MeasureRow(verticalAlignment ?? new BiasAlignment(0, -1)));

    public static void Spacer(ScopeModifier? spacerModifier = default) =>
        Layout((spacerModifier?.SelfModifier ?? IModifier.Empty).Then(new DebugModifier {ScopeName = nameof(Spacer)}), null,
            BoxLayout.Measure(new BiasAlignment(-1, -1)));
}
