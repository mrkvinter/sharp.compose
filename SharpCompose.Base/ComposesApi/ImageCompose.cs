using SharpCompose.Base.Modifiers;
using SharpCompose.Base.Modifiers.DrawableModifiers;
using SharpCompose.Drawer.Core.Images;

namespace SharpCompose.Base.ComposesApi;

public partial class BaseCompose
{
    public static void Icon(IImage image, Modifier? modifier = null)
    {
        IScopeModifier<Modifier> scope = modifier ?? Modifier.With;
        Box(scope
            .Then(new CombinedModifier(
                new ImageDrawingModifier(image), 
                new ImageMeasureModifier(image)))
        );
    }
}