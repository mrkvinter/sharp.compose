using SharpCompose.Base.Layouting;
using SharpCompose.Base.Modifiers.DrawableModifiers;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Utilities;

namespace SharpCompose.Base.Modifiers;

public sealed class TextDrawModifier : IDrawableLayerModifier
{
    public string Text { get; }
    public Brush Brush => brush;

    private readonly double fontSize;
    private readonly Font font;
    private readonly TextAlignment textAlignment;
    private readonly Brush brush;

    public TextDrawModifier(string text, double fontSize, Font font, TextAlignment textAlignment, Brush brush)
    {
        Text = text;
        this.fontSize = fontSize;
        this.font = font;
        this.textAlignment = textAlignment;
        this.brush = brush;
    }

    public Measurable Introduce(Measurable measurable, IGraphics graphics) => 
        measurable with
        {
            Measure = constraints =>
            {
                measurable.Measure(constraints);
                var measureText = graphics.MeasureText(Text, fontSize, font, textAlignment, new IntSize(constraints.MaxWidth, constraints.MaxHeight));

                return new MeasureResult
                {
                    Width = constraints.ClampWidth(measureText.Width), 
                    Height = constraints.ClampHeight(measureText.Height), 
                    Placeable = (x, y) =>
                        graphics.DrawText(Text, fontSize, font, brush, new IntOffset(x, y), textAlignment,
                            new IntSize(constraints.MaxWidth, constraints.MaxHeight))
                };
            }
        };
}