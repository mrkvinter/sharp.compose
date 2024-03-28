using System.Threading.Tasks;
using NUnit.Framework;
using SharpCompose.Base;
using SharpCompose.Base.Extensions;
using TestSharpCompose.ComposeTester;

namespace TestSharpCompose;

public static class ExamplesTests
{
    [Test]
    public static void ReadmeExample_Counter_CounterDisplayed()
    {
        var composeTester = new ComposeTester.ComposeTester(Counter);

        var node = composeTester.Root.OnNodeWithText("Current count: 0");
        var buttonNode = composeTester.Root.OnNodeWithText("Click me");

        Assert.IsNotNull(node);
        Assert.IsNotNull(buttonNode);
    }
    
    [Test]
    public static async Task ReadmeExample_Counter_ClickButton_CountIncreased()
    {
        var composeTester = new ComposeTester.ComposeTester(Counter);

        composeTester.Root.OnNodeWithText("Click me")!.PerformClick();
        await composeTester.RecomposeAsync();

        var node = composeTester.Root.OnNodeWithText("Current count: 1");
        Assert.IsNotNull(node);
    }
    
    [Composable]
    private static void Counter()
    {
        Column(content: () =>
        {
            var counter = Remember.Get(() => 0.AsMutableState());

            Text($"Current count: {counter.Value}");
            Button(() => counter.Value++, "Click me");
        });
    }
}