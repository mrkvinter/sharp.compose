namespace SharpCompose.Base.Modifiers;

public interface IComponentModifier
{
    Action<Composer.Scope>? MetaProducer { get; set; }
}

public interface ISizeModifier
{
}

public interface IPaddingModifier
{
}

public interface IMarginModifier
{
}

public class LayoutModifier : IComponentModifier,
    ISizeModifier, IBackgroundModifier, ICornersModifier, IBorderModifier,
    IPaddingModifier, IMarginModifier, IShadowModifier
{
    public static LayoutModifier With => new();

    public Action<Composer.Scope>? MetaProducer { get; set; }
}

public sealed class BoxModifier : LayoutModifier
{
    public new static BoxModifier With => new();
}

public sealed class RowModifier : LayoutModifier
{
    public new static RowModifier With => new();
}

public sealed class ColumnModifier : LayoutModifier
{
    public new static ColumnModifier With => new();
}

public sealed class SpacerModifier : IComponentModifier, ISizeModifier
{
    public static SpacerModifier With => new();

    public Action<Composer.Scope>? MetaProducer { get; set; }
}

public static class LayoutModifierExtensions
{
    public static T Size<T>(this T self, int width, int height) where T : IComponentModifier, ISizeModifier
    {
        self.MetaProducer += scope => scope.AddMeta<(int?, int?)>("started.size", (width, height));

        return self;
    }

    public static T Width<T>(this T self, int width) where T : IComponentModifier, ISizeModifier
    {
        self.MetaProducer += scope =>
        {
            var oldSize = scope.GetMetaByKey<(int? w, int? h)?>("started.size") ?? (null, null);
            scope.AddMeta<(int?, int?)>("started.size", (width, oldSize.h));
        };

        return self;
    }

    public static T Height<T>(this T self, int height) where T : IComponentModifier, ISizeModifier
    {
        self.MetaProducer += scope =>
        {
            var oldSize = scope.GetMetaByKey<(int? w, int? h)?>("started.size") ?? (null, null);
            scope.AddMeta<(int?, int?)>("started.size", (oldSize.w, height));
        };

        return self;
    }

    public static T Padding<T>(this T self, int padding) where T : IComponentModifier, IPaddingModifier
    {
        self.MetaProducer += scope => scope.AddMeta("padding", new Vector4Int
        {
            Start = padding, Top = padding, End = padding, Bottom = padding
        });

        return self;
    }

    public static T Margin<T>(this T self, int margin) where T : IComponentModifier, IMarginModifier
    {
        self.MetaProducer += scope => scope.AddMeta("margin", new Vector4Int
        {
            Start = margin, Top = margin, End = margin, Bottom = margin
        });

        return self;
    }

    public static T MarginTop<T>(this T self, int margin) where T : IComponentModifier, IMarginModifier
    {
        self.MetaProducer += scope =>
        {
            var oldMargin = scope.GetMetaByKey<Vector4Int?>("margin") ?? new Vector4Int();

            scope.AddMeta("margin", oldMargin with {Top = margin});
        };

        return self;
    }

    public static T MarginStart<T>(this T self, int margin) where T : IComponentModifier, IMarginModifier
    {
        self.MetaProducer += scope =>
        {
            var oldMargin = scope.GetMetaByKey<Vector4Int?>("margin") ?? new Vector4Int();

            scope.AddMeta("margin", oldMargin with {Start = margin});
        };

        return self;
    }
}

public struct Vector4Int
{
    public int Start;
    public int Top;
    public int End;
    public int Bottom;
}