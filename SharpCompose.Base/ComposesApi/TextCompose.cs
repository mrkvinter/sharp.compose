using System.Drawing;
using SharpCompose.Base.ComposesApi.Providers;
using SharpCompose.Base.Layouting;
using SharpCompose.Base.Modifiers;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;

namespace SharpCompose.Base.ComposesApi;

public static partial class BaseCompose
{
    public static readonly Func<string, double, Font, (int w, int h)> MeasureText = (_, _, _) => (0, 0);

    private static (int w, int h) CalculateVisualSize(string text, double size, Font font)
        => MeasureText(text, size, font);

    public static void Text(string text, int? size = null, Color? color = default, Font? font = null) =>
        TextElement(text, 
            size ?? LocalProviders.TextStyle.Value.FontSize, 
            color ?? LocalProviders.TextStyle.Value.Color ?? LocalProviders.Colors.Value.OnStandard, 
            font ?? LocalProviders.TextStyle.Value.Font);

    private static void TextElement(string text, double size, Color color, Font font)
    {
        MeasureResult MeasureText(Measurable[] _, Constraints constraints)
        {
            var (width, height) = CalculateVisualSize(text, size, font);

            return new MeasureResult
            {
                Width = width,
                Height = height,
                Placeable = (_, _) => { }
            };
        }

        IModifier modifier = new TextDrawModifier(text, size, font, new SolidColorBrush(color));

        Composer.Instance.StartScope(modifier.Then(new DebugModifier {ScopeName = nameof(TextElement)}), MeasureText);
        Composer.Instance.StopScope();
    }
}