using System;
using SharpCompose.Base;
using SharpCompose.Base.ElementBuilder;
using SharpCompose.Base.Modifiers;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;

namespace SharpCompose.Win.ElementBuilders;

public enum Direction
{
    None,
    Vertical,
    Horizontal
}

public class LayoutBuilder : IElementBuilder
{
    public static LayoutBuilder Instance { get; } = new();

    private LayoutBuilder()
    {
    }

    public (int w, int h) CalculateVisualSize(Composer.Scope scope, ICanvas canvas)
    {
        var size = scope.GetMetaByKey<(int, int)?>("visual.size");
        if (size == null)
        {
            var startedSize = scope.GetMetaByKey<(int? w, int? h)?>("started.size");
            if (startedSize == null || startedSize.Value.w == null || startedSize.Value.h == null)
            {
                var direction = scope.GetMeta<Direction>();
                var spacingBetween = scope.GetMetaByKey<int>("spacing");

                var (width, height) = (0, 0);
                var count = scope.Child.Count;
                var index = 0;
                foreach (var scopeChild in scope.Child)
                {
                    index++;
                    var childSize = scopeChild.ElementBuilder.CalculateRealSize(scopeChild, canvas);
                    (width, height) = MeasureSize(scope, direction, spacingBetween, (width, height), childSize, count == index);
                }

                var padding = scope.GetMetaByKey<Vector4Int?>("padding") ?? new Vector4Int();
                size = (
                    startedSize?.w ?? width + padding.Start + padding.End,
                    startedSize?.h ?? height + padding.Top + padding.Bottom);
            }
            else
                size = (startedSize.Value.w.Value, startedSize.Value.h.Value);

            scope.AddMeta("visual.size", size);
        }

        return size.Value;
    }

    public (int w, int h) CalculateRealSize(Composer.Scope scope, ICanvas canvas)
    {
        var realSize = scope.GetMetaByKey<(int, int)?>("real.size");
        if (realSize == null)
        {
            var size = scope.ElementBuilder.CalculateVisualSize(scope, canvas);
            var margin = scope.GetMetaByKey<Vector4Int?>("margin") ?? new Vector4Int();

            realSize = (size.w + margin.Start + margin.End, size.h + margin.Top + margin.Bottom);
            scope.AddMeta("real.size", size);
        }

        return realSize.Value;
    }

    public void Draw(Composer.Scope scope, ICanvas canvas, int pointerX = 0, int pointerY = 0)
    {
        var (width, height) = CalculateVisualSize(scope, canvas);

        (pointerX, pointerY) = MeasureStartedPointer(scope, (pointerX, pointerY));

        canvas.DrawRectangle(
            new Rect(pointerX, pointerY, width, height),
            scope.TryGetMeta<Corners>(out var corner) ? corner : null,
            scope.GetMeta<Border>(),
            scope.GetMetaByKey<Brush>("background") ?? SolidColorBrush.Transparent,
            scope.TryGetMeta<ShadowInfo>(out var shadowInfo) ? shadowInfo : null);

        (pointerX, pointerY) = MeasureContentStartedPointer(scope, (pointerX, pointerY));
        var direction = scope.GetMeta<Direction>();
        var spacingBetween = scope.GetMetaByKey<int>("spacing");

        var index = 0;
        foreach (var childScope in scope.Child)
        {
            index++;
            childScope.ElementBuilder.Draw(childScope, canvas, pointerX, pointerY);

            (pointerX, pointerY) = MeasurePointer(direction, spacingBetween, (pointerX, pointerY),
                childScope.ElementBuilder.CalculateRealSize(childScope, canvas), scope.Child.Count == index);
        }
    }

    private static (int w, int h) MeasureStartedPointer(Composer.Scope scope, (int x, int y) pointer)
    {
        var margin = scope.GetMetaByKey<Vector4Int?>("margin") ?? new Vector4Int();

        return (pointer.x + margin.Start, pointer.y + margin.Top);
    }

    private static (int w, int h) MeasureContentStartedPointer(Composer.Scope scope, (int x, int y) pointer)
    {
        var padding = scope.GetMetaByKey<Vector4Int?>("padding") ?? new Vector4Int();

        return (pointer.x + padding.Start, pointer.y + padding.Top);
    }

    private static (int w, int h) MeasureSize(Composer.Scope scope, Direction direction, int spacingBetween,
        (int w, int h) currentSize, (int w, int h) childSize,
        bool isLastChild) =>
        (CalculateWidth(scope, direction, spacingBetween, currentSize, childSize, isLastChild),
            CalculateHeight(scope, direction, spacingBetween, currentSize, childSize, isLastChild));

    private static (int x, int y) MeasurePointer(Direction direction, int spacingBetween, (int x, int y) currentPointer,
        (int w, int h) childSize, bool isLastChild) =>
        direction switch
        {
            Direction.None => currentPointer,

            Direction.Vertical => (currentPointer.x,
                currentPointer.y + childSize.h + (isLastChild ? 0 : spacingBetween)),

            Direction.Horizontal => (currentPointer.x + childSize.w + (isLastChild ? 0 : spacingBetween),
                currentPointer.y),

            _ => throw new NotImplementedException()
        };

    private static int CalculateWidth(Composer.Scope scope, Direction direction, int spacingBetween,
        (int w, int h) currentSize, (int w, int h) childSize,
        bool isLastChild)
    {
        var size = scope.GetMetaByKey<(int? w, int? h)?>("startedSize") ?? (null, null);
        if (size.w.HasValue)
            return size.w.Value;

        return direction switch
        {
            Direction.None => Math.Max(currentSize.w, childSize.w),

            Direction.Vertical => Math.Max(currentSize.w, childSize.w),

            Direction.Horizontal => currentSize.w + childSize.w + (isLastChild ? 0 : spacingBetween),

            _ => throw new NotImplementedException()
        };
    }

    private static int CalculateHeight(Composer.Scope scope, Direction direction, int spacingBetween,
        (int w, int h) currentSize, (int w, int h) childSize,
        bool isLastChild)
    {
        var size = scope.GetMetaByKey<(int? w, int? h)?>("started.size") ?? (null, null);
        if (size.h.HasValue)
            return size.h.Value;

        return direction switch
        {
            Direction.None => Math.Max(currentSize.h, childSize.h),

            Direction.Vertical => currentSize.h + childSize.h + (isLastChild ? 0 : spacingBetween),

            Direction.Horizontal => Math.Max(currentSize.h, childSize.h),

            _ => throw new NotImplementedException()
        };
    }

    public override string ToString() => $"{nameof(LayoutBuilder)}";
}