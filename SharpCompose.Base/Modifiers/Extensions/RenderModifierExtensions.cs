using System.Drawing;
using SharpCompose.Base.Modifiers.DrawableModifiers;
using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Shapes;

namespace SharpCompose.Base.Modifiers.Extensions;

public static class RenderModifierExtensions
{
    public static T BackgroundColor<T>(this T self, Color color) where T : IScopeModifier<T>
        => self.Then(new BackgroundModifier(new SolidColorBrush(color)));

    public static T Clip<T>(this T self, IShape shape) where T : IScopeModifier<T>
        => self.Then(new ClippingModifier(shape));

    public static T BackgroundGradientColor<T>(this T self, Color firstColor, Color secondColor,
        (float, float) firstPoint, (float, float) secondPoint) where T : IScopeModifier<T>
        => self.Then(new BackgroundModifier(new LinearGradientBrush(firstColor, secondColor, firstPoint, secondPoint)));

    public static T Border<T>(this T self, Color color, int width, IShape shape)
        where T : IScopeModifier<T>
        => self.Then(new BorderModifier(width, new SolidColorBrush(color), shape));

    public static T Border<T>(this T self, Brush brush, int width, IShape shape)
        where T : IScopeModifier<T>
        => self.Then(new BorderModifier(width, brush, shape));

    public static T Shadow<T>(this T self, Color? color, (int x, int y)? offset = null, int blurRadius = 0, IShape? shape = null)
        where T : IScopeModifier<T>
        => self.Then(new ShadowModifier(offset ?? (0, 0), blurRadius, new SolidColorBrush(color ?? Color.Black), shape ?? Shapes.Rectangle));
}