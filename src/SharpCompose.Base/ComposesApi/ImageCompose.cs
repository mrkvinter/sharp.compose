using SharpCompose.Base.Modifiers;
using SharpCompose.Base.Modifiers.DrawableModifiers;
using SharpCompose.Drawer.Core.Images;

namespace SharpCompose.Base.ComposesApi;

public partial class BaseCompose
{
    public static void Icon(IImage image, ScopeModifier? modifier = null)
    {
        IScopeModifier<ScopeModifier> scope = modifier ?? Modifier;
        Box(scope
            .Then(new CombinedModifier(
                new ImageDrawingModifier(image), 
                new ImageMeasureModifier(image)))
        );
    }
}