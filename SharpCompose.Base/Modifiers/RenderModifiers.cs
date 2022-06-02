using System.Drawing;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;

namespace SharpCompose.Base.Modifiers;

public interface IBackgroundModifier
{
}

public interface IShadowModifier
{
}

public interface ICornersModifier
{
}

public interface IBorderModifier
{
}

public static class RenderModifierExtensions
{
    public static T BackgroundColor<T>(this T self, Color color) where T : IComponentModifier, IBackgroundModifier
    {
        self.MetaProducer += s => s.AddMeta<Brush>("background",
            new SolidColorBrush(color));

        return self;
    }

    public static T BackgroundGradientColor<T>(this T self, Color firstColor, Color secondColor,
        (float, float) firstPoint, (float, float) secondPoint)
        where T : IComponentModifier, IBackgroundModifier
    {
        self.MetaProducer += s =>
            s.AddMeta<Brush>("background", new LinearGradientBrush(firstColor, secondColor, firstPoint, secondPoint));

        return self;
    }

    public static T Corners<T>(this T self, int corners) where T : IComponentModifier, ICornersModifier
    {
        self.MetaProducer += s => s.AddMeta(new Corners(corners, corners, corners, corners));

        return self;
    }

    public static T Corners<T>(this T self, int topLeft, int topRight, int bottomLeft, int bottomRight)
        where T : IComponentModifier, ICornersModifier
    {
        self.MetaProducer += s => s.AddMeta(new Corners(topLeft, topRight, bottomLeft, bottomRight));

        return self;
    }

    public static T Border<T>(this T self, Color color, int width) where T : IComponentModifier, IBorderModifier
    {
        self.MetaProducer += s => s.AddMeta(new Border(new SolidColorBrush(color), width));

        return self;
    }

    public static T Shadow<T>(this T self, Color color, (int x, int y) offset, float blurRadius = 0)
        where T : IComponentModifier, IShadowModifier
    {
        self.MetaProducer += s => s.AddMeta(new ShadowInfo(color, offset, blurRadius));

        return self;
    }
}