using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SharpCompose.Base;
using SharpCompose.Base.ComposesApi.Providers;
using SharpCompose.Base.Extensions;
using SharpCompose.Base.Modifiers;
using SharpCompose.Base.Modifiers.Extensions;
using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Utilities;
using TestSharpCompose.ComposeTester;

namespace TestSharpCompose;

public class BoxWithConstraintsTests
{
    [Test]
    public async Task BoxWithConstraints_ChangeSize_ShowinTextForSmallContent()
    {
        var composeTester = new ComposeTester.ComposeTester(AdaptiveContent);

        await composeTester.RecomposeAsync();
        composeTester.Size = new IntSize(300, 100);
        await composeTester.RecomposeAsync();
        var smallContentNode = composeTester.Root.OnNodeWithText("Small content");
        var largeContentNode = composeTester.Root.OnNodeWithText("Large content");

        Assert.That(smallContentNode, Is.Not.Null);
        Assert.That(largeContentNode, Is.Null);
    }

    [Test]
    public async Task BoxWithConstraints_LocalProviders_CorrectLocalProviders()
    {
        var composeTester = new ComposeTester.ComposeTester(AdaptiveContentWithUsingProvider);

        await composeTester.RecomposeAsync();
        composeTester.Size = new IntSize(300, 100);
        await composeTester.RecomposeAsync();
        var smallContentNode = composeTester.Root.OnNodeWithText("Small content test-text");
        var largeContentNode = composeTester.Root.OnNodeWithText("Large content test-text");

        Assert.That(smallContentNode, Is.Not.Null);
        Assert.That(largeContentNode, Is.Null);
    }

    [Test]
    public async Task BoxWithConstraints_LocalProvidersWithThemes_CorrectLocalProviders()
    {
        var composeTester = new ComposeTester.ComposeTester(AdaptiveContentWithUsingProviderAndDarkTheme);

        await composeTester.RecomposeAsync();
        composeTester.Size = new IntSize(300, 100);
        await composeTester.RecomposeAsync();
        var smallContentNode = composeTester.Root.OnNodeWithModifier<TextDrawModifier>(e => e.Text =="Small content test-text")!;
        var largeContentNode = composeTester.Root.OnNodeWithText("Large content test-text");
        var textDrawModifier = (TextDrawModifier) smallContentNode.LayoutNode.Modifier
            .SqueezeModifiers().First(e => e is TextDrawModifier);
        var smallContentColor = ((SolidColorBrush) textDrawModifier.Brush).Color;

        Assert.That(smallContentColor, Is.EqualTo(Color.Black));
        Assert.That(largeContentNode, Is.Null);
    }

    [Test]
    public async Task BoxWithConstraints_LocalProvidersWithChangedTheme_CorrectLocalProviders()
    {
        var composeTester = new ComposeTester.ComposeTester(AdaptiveContentWithUsingProviderAndDarkTheme);

        await composeTester.RecomposeAsync();
        composeTester.Size = new IntSize(300, 100);
        composeTester.Root.OnNodeWithId("ChangeThemeButton")!.PerformClick();
        await composeTester.RecomposeAsync();
        var smallContentNode = composeTester.Root.OnNodeWithModifier<TextDrawModifier>(e => e.Text =="Small content test-text")!;
        var largeContentNode = composeTester.Root.OnNodeWithText("Large content test-text");
        var textDrawModifier = (TextDrawModifier) smallContentNode.LayoutNode.Modifier
            .SqueezeModifiers().First(e => e is TextDrawModifier);
        var textColor = ((SolidColorBrush) textDrawModifier.Brush).Color;

        Assert.That(textColor, Is.EqualTo(Color.White));
        Assert.That(largeContentNode, Is.Null);
    }

    [Composable]
    private static void AdaptiveContent() => BoxWithConstraints(content: constraints =>
    {
        if (constraints.MaxWidth < 500)
            Box(content: () => Text("Small content"));
        else
            Box(content: () => Text("Large content"));
    });

    private static readonly LocalProvider<string> LocalTestTextProvider = new(string.Empty);

    [Composable]
    private static void AdaptiveContentWithUsingProvider() => CompositionLocalProvider(new[]
    {
        LocalTestTextProvider.Provide("test-text"),
    }, content: () => BoxWithConstraints(content: constraints =>
    {
        if (constraints.MaxWidth < 500)
            Box(content: () => Text($"Small content {LocalTestTextProvider.Value}"));
        else
            Box(content: () => Text($"Large content {LocalTestTextProvider.Value}"));
    }));


    [Composable]
    private static void AdaptiveContentWithUsingProviderAndDarkTheme() => Column(content: () =>
    {
        var isLight = Remember.Get(() => true.AsMutableState());
        var darkTheme = new Colors(Color.Navy, Color.White, Color.Black, Color.White, Color.Black);
        var lightTheme = new Colors(Color.Aqua, Color.Black, Color.White, Color.Black, Color.White);
        CompositionLocalProvider(new[]
        {
            LocalTestTextProvider.Provide("test-text"),
            LocalColors.Provide(isLight.Value ? lightTheme : darkTheme)
        }, content: () => Box(Modifier.BackgroundColor(LocalColors.Value.Background), content: () =>
        {
            Button(() => isLight.Value = !isLight.Value, "Change Theme", Modifier.Id("ChangeThemeButton"));
            BoxWithConstraints(content: constraints =>
            {
                if (constraints.MaxWidth < 500)
                    Box(content: () => Text($"Small content {LocalTestTextProvider.Value}"));
                else
                    Box(content: () => Text($"Large content {LocalTestTextProvider.Value}"));
            });
        }));
    });
}