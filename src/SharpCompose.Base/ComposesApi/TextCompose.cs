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
            LocalTextStyle.Value with
            {
                FontSize = size ?? LocalTextStyle.Value.FontSize,
                Font = font ?? LocalTextStyle.Value.Font
            },
            color ?? LocalTextStyle.Value.Color ?? LocalColors.Value.OnStandard,
            textAlignment ?? TextAlignment.Left);

    private static void TextElement(string text, IModifier modifier, TextStyle textStyle, Color color, TextAlignment textAlignment)
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

        var textDrawerModifier = Remember.Get(text, textStyle, color,
            () => new TextDrawModifier(text, textStyle.FontSize, textStyle.Font, textAlignment, new SolidColorBrush(color)));
        
        Composer.Instance.StartScope(
            modifier
                .Then(textDrawerModifier)
                .Then(new DebugModifier {ScopeName = nameof(TextElement)}), MeasureText);
        Composer.Instance.StopScope();
    }
}