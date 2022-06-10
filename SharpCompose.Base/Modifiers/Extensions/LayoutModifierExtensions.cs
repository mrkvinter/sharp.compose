using SharpCompose.Base.Layouting;
using SharpCompose.Base.Modifiers.LayoutModifiers;

namespace SharpCompose.Base.Modifiers.Extensions;

public static class LayoutModifierExtensions
{
    public static T Size<T>(this T self, int width, int height) where T : IScopeModifier<T>
        => self.Then(new SizeModifier(Constraints.Fixed(width, height)));

    public static T Width<T>(this T self, int width) where T : IScopeModifier<T>
        => self.Then(new SizeModifier(Constraints.FixedWidth(width)));

    public static T Height<T>(this T self, int height) where T : IScopeModifier<T>
        => self.Then(new SizeModifier(Constraints.FixedHeight(height)));

    public static T Padding<T>(this T self, int padding) where T : IScopeModifier<T>
        => self.Then(new PaddingModifier(padding, padding, padding, padding));

    public static T Padding<T>(this T self, int horizontal = 0, int vertical = 0)
        where T : IScopeModifier<T>
        => self.Then(new PaddingModifier(horizontal, vertical, horizontal, vertical));

    public static T Padding<T>(this T self, int start = 0, int top = 0, int end = 0, int bottom = 0)
        where T : IScopeModifier<T>
        => self.Then(new PaddingModifier(start, top, end, bottom));

    public static T FillMaxWidth<T>(this T self) where T : IScopeModifier<T>
        => self.Then(new FillModifier(Direction.Horizontal));

    public static T FillMaxHeight<T>(this T self) where T : IScopeModifier<T>
        => self.Then(new FillModifier(Direction.Vertical));

    public static T MinSize<T>(this T self, int width, int height) where T : IScopeModifier<T>
        => self.Then(new SizeModifier(Constraints.MinSize(width, height)));

    public static T MatchParentWidth<T>(this T self) where T : IScopeModifier<T>
        => self.Then(new ParentDataModifier<BoxParentData>(e =>
            (e ?? new BoxParentData(false, false)) with {MatchParentWidth = true}));

    public static T MatchParentHeight<T>(this T self) where T : IScopeModifier<T>
        => self.Then(new ParentDataModifier<BoxParentData>(e =>
            (e ?? new BoxParentData(false, false)) with {MatchParentHeight = true}));

    public static T MatchParentSize<T>(this T self) where T : IScopeModifier<T>
        => self.Then(new ParentDataModifier<BoxParentData>(e =>
            // ReSharper disable once WithExpressionModifiesAllMembers
            (e ?? new BoxParentData(false, false)) with
            {
                MatchParentWidth = true, MatchParentHeight = true
            }));
}