using System;
using SharpCompose.Base;
using SharpCompose.Base.Modifiers;
using SharpCompose.Win.ElementBuilders;

namespace SharpCompose.Win;

public static class WinCompose
{
    private static void Layout(Direction direction, int spacingBetween, LayoutModifier layoutModifier, Action? content)
    {
        void Factory(Composer _)
        {
        }

        layoutModifier.MetaProducer += scope =>
        {
            scope.AddMeta(direction);
            scope.AddMeta("spacing", spacingBetween);
        };

        Composer.Instance.StartScope(Factory, layoutModifier, LayoutBuilder.Instance);
        content?.Invoke();
        Composer.Instance.StopScope();
    }

    public static void Box(BoxModifier? boxModifier = default, Action? content = null) =>
        Layout(Direction.None, 0, boxModifier ?? BoxModifier.With, content);

    public static void
        Column(ColumnModifier? columnModifier = default, int spacingBetween = 0, Action? content = null) =>
        Layout(Direction.Vertical, spacingBetween, columnModifier ?? ColumnModifier.With, content);

    public static void Row(RowModifier? rowModifier = default, int spacingBetween = 0, Action? content = null) =>
        Layout(Direction.Horizontal, spacingBetween, rowModifier ?? RowModifier.With, content);

    public static void Spacer(SpacerModifier? spacerModifier = default)
    {
        void Factory(Composer _)
        {
        }

        spacerModifier ??= SpacerModifier.With;
        spacerModifier.MetaProducer += scope =>
        {
            scope.AddMeta(Direction.None);
            scope.AddMeta("spacing", 0);
        };
        Composer.Instance.StartScope(Factory, spacerModifier, LayoutBuilder.Instance);
        Composer.Instance.StopScope();
    }
}