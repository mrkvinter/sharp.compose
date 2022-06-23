using SharpCompose.Base.Layouting;
using SharpCompose.Drawer.Core;

namespace SharpCompose.Base.Modifiers.DrawableModifiers;

public interface IDrawableLayerModifier : IModifier.IElement
{
    Measurable Introduce(Measurable measurable, IGraphics graphics);
}