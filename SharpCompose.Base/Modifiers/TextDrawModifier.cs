using SharpCompose.Base.Layouting;
using SharpCompose.Base.Modifiers.DrawableModifiers;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;

namespace SharpCompose.Base.Modifiers;

public sealed class TextDrawModifier : IDrawableLayerModifier
{
    public string Text { get; }

    private readonly double fontSize;
    private readonly Font font;
    private readonly Brush brush;

    public TextDrawModifier(string text, double fontSize, Font font, Brush brush)
    {
        Text = text;
        this.fontSize = fontSize;
        this.font = font;
        this.brush = brush;
    }

    public Measurable Introduce(Measurable measurable, IGraphics graphics) => 
        measurable with
        {
            Measure = constraints =>
            {
                var measureResult = measurable.Measure(constraints);
                var textSize = graphics.MeasureText(Text, fontSize, font);

                return new MeasureResult {Width = textSize.w, Height = textSize.h, Placeable = (x, y) =>
                {
                    graphics.DrawText(Text, fontSize, font, brush, x, y);
                    measureResult.Placeable(x, y);
                }};
            }
        };
}