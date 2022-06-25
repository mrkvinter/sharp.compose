using SharpCompose.Base.ComposesApi.Providers;
using SharpCompose.Base.Layouting;
using SharpCompose.Base.Modifiers;

namespace SharpCompose.Base.ComposesApi;

public partial class BaseCompose
{
    public static ScopeModifier Modifier => new();

    public static void VoidScope(Action content)
    {
        Composer.Instance.StartScope(IModifier.Empty.Then(new DebugModifier {ScopeName = nameof(VoidScope)}),
            BoxLayout.Measure(new BiasAlignment(0, 0)));
        content();
        Composer.Instance.StopScope();
    }

    public static void For<T>(IEnumerable<T> enumerable, Action<T> itemContent) where T : notnull
    {
        using var indexController = Remember.StartLoopIndex();
        foreach (var item in enumerable)
        {
            indexController.Next(item.GetHashCode());
            itemContent(item);
        }
    }

    public static void CompositionLocalProvider(Provider[] providers, Action content)
    {
        foreach (var provider in providers)
        {
            provider.StartProvide();
        }

        content();
        foreach (var provider in providers)
        {
            provider.EndProvide();
        }
    }
}