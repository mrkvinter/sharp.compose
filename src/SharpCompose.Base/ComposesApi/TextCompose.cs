using System.Drawing;
using SharpCompose.Base.ComposesApi.Providers;
using SharpCompose.Base.Layouting;
using SharpCompose.Base.Modifiers;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Utilities;

namespace SharpCompose.Base.ComposesApi;

public static partial class BaseCompose
{
    public static void Text(string text, ScopeModifier? modifier = null, int? size = null, Color? color = default, Font? font = null, TextAlignment? textAlignment = null) =>
        TextElement(text, 
            modifier?.SelfModifier ?? IModifier.Empty,
            size ?? LocalProviders.TextStyle.Value.FontSize, 
            color ?? LocalProviders.TextStyle.Value.Color ?? LocalProviders.Colors.Value.OnStandard, 
            font ?? LocalProviders.TextStyle.Value.Font,
            textAlignment ?? TextAlignment.Left);

    private static void TextElement(string text, IModifier modifier, double size, Color color, Font font, TextAlignment textAlignment)
    {
        MeasureResult MeasureText(Measurable[] _, Constraints constraints)
        {
            return new MeasureResult
            {
                Width = 0,
                Height = 0,
                Placeable = (_, _) => { }
            };
        }

        Composer.Instance.StartScope(
            modifier
                .Then(new TextDrawModifier(text, size, font, textAlignment, new SolidColorBrush(color)))
                .Then(new DebugModifier {ScopeName = nameof(TextElement)}), MeasureText);
        Composer.Instance.StopScope();
    }
}