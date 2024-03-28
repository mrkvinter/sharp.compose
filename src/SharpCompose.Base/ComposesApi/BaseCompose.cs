using SharpCompose.Base.ComposesApi.Providers;
using SharpCompose.Base.Modifiers;

namespace SharpCompose.Base.ComposesApi;

public partial class BaseCompose
{
    public static ScopeModifier Modifier => new();

    [Composable]
    public static void CompositionLocalProvider(Provider[] providers, Action content)
    {
        Composer.Instance.StartGroup(() =>
        {
            Remember.LaunchedEffect(true, () =>
            {
                var currentGroup = Composer.Instance.CurrentGroup;
                currentGroup.Locals = new Dictionary<int, object>(currentGroup.Locals);
            });
            
            foreach (var provider in providers)
            {
                provider.StartProvide();
            }

            content();
        });
        
        Composer.Instance.EndGroup();
    }
}