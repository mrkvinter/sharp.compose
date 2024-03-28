using SharpCompose.Base.Modifiers;
using SharpCompose.Base.Modifiers.DrawableModifiers;
using SharpCompose.Drawer.Core.Images;

namespace SharpCompose.Base.ComposesApi;

public partial class BaseCompose
{
    [UiComposable]
    public static void Icon(IImage image, ScopeModifier? modifier = null)
    {
        IScopeModifier<ScopeModifier> scope = modifier ?? Modifier;
        Box(scope
#if DEBUG
            .Then(new DebugModifier {ScopeName = nameof(Icon)})
#endif
            .Then(new CombinedModifier(
                new ImageDrawingModifier(image), 
                new ImageMeasureModifier(image)))
        );
    }
}