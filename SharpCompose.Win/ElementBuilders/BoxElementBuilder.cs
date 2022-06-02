using System;
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
    private readonly IShadowModifier? shadowModifier;
    private readonly Direction direction;
    private readonly int spacingBetween;
    private readonly int? width;
    private readonly int? height;
    private readonly Brush? background;
    private readonly Vector4Int margin;
    private readonly Vector4Int padding;

    public Brush Background => background ?? SolidColorBrush.Transparent;

    public Corners? Corners { get; }

    public Border Border { get; }

    public ShadowInfo? ShadowInfo => shadowModifier != null
        ? new ShadowInfo(shadowModifier.Color, shadowModifier.Offset, shadowModifier.BlurRadius)
        : null;

    public LayoutBuilder(
        Direction direction,
        int spacingBetween,
        ISizeModifier? sizeModifier,
        IBackgroundModifier? backgroundModifier,
        ICornersModifier? cornersModifier, IBorderModifier? borderModifier,
        IPaddingModifier? paddingModifier, IMarginModifier? marginModifier,
        IShadowModifier? shadowModifier)
    {
        this.shadowModifier = shadowModifier;
        this.direction = direction;
        this.spacingBetween = spacingBetween;
        (width, height) = sizeModifier?.SizeValue ?? default;
        background = backgroundModifier?.BackgroundBrushValue;
        Corners = cornersModifier?.CornersValue;
        Border = borderModifier?.Border ?? default;
        padding = paddingModifier?.Padding ?? default;
        margin = marginModifier?.Margin ?? default;
    }

    public (int w, int h) MeasureStartedSize()
    {
        var startedWidth = width ?? 0;
        var startedHeight = height ?? 0;

        return (startedWidth + padding.Start + padding.End, startedHeight + padding.Top + padding.Bottom);
    }

    public (int w, int h) MeasureExtendSize((int w, int h) size)
    {
        return (size.w + margin.Start + margin.End, size.h + margin.Top + margin.Bottom);
    }

    public (int w, int h) MeasureStartedPointer((int x, int y) pointer)
    {
        return (pointer.x + margin.Start, pointer.y + margin.Top);
    }

    public (int w, int h) MeasureContentStartedPointer((int x, int y) pointer)
    {
        return (pointer.x + padding.Start, pointer.y + padding.Top);
    }

    public (int w, int h) MeasureSize((int w, int h) currentSize, (int w, int h) childSize, bool isLastChild) =>
        (CalculateWidth(currentSize, childSize, isLastChild),
            CalculateHeight(currentSize, childSize, isLastChild));

    public (int x, int y) MeasurePointer((int x, int y) currentPointer, (int w, int h) childSize, bool isLastChild) =>
        direction switch
        {
            Direction.None => currentPointer,

            Direction.Vertical => (currentPointer.x,
                currentPointer.y + childSize.h + (isLastChild ? 0 : spacingBetween)),

            Direction.Horizontal => (currentPointer.x + childSize.w + (isLastChild ? 0 : spacingBetween),
                currentPointer.y),

            _ => throw new NotImplementedException()
        };

    private int CalculateWidth((int w, int h) currentSize, (int w, int h) childSize, bool isLastChild)
    {
        if (width.HasValue)
            return width.Value;

        return direction switch
        {
            Direction.None => Math.Max(currentSize.w, childSize.w + padding.Start + padding.End),

            Direction.Vertical => Math.Max(currentSize.w, childSize.w + padding.Start + padding.End),

            Direction.Horizontal => currentSize.w + childSize.w + (isLastChild ? 0 : spacingBetween),

            _ => throw new NotImplementedException()
        };
    }

    private int CalculateHeight((int w, int h) currentSize, (int w, int h) childSize, bool isLastChild)
    {
        if (height.HasValue)
            return height.Value;

        return direction switch
        {
            Direction.None => Math.Max(currentSize.h, childSize.h + padding.Top + padding.Bottom),

            Direction.Vertical => currentSize.h + childSize.h + (isLastChild ? 0 : spacingBetween),

            Direction.Horizontal => Math.Max(currentSize.h, childSize.h + padding.Top + padding.Bottom),

            _ => throw new NotImplementedException()
        };
    }

    public override string ToString() => $"Layout [{direction.ToString()}, {spacingBetween}px]";
}