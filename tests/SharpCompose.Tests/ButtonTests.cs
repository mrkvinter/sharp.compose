using NUnit.Framework;
using TestSharpCompose.ComposeTester;

namespace TestSharpCompose;

public class ButtonTests
{
    [Test]
    public void ButtonWithText_PerformClick_StateChanged()
    {
        var state = false;
        var composeTester = new ComposeTester.ComposeTester(() => { Button(() => state = true, "Click"); });

        composeTester.Root.OnNodeWithText("Click")!.PerformClick();

        Assert.IsTrue(state);
    }

    [Test]
    public void ButtonInsideLoop_Composing_NoException()
    {
        var composeTester = new ComposeTester.ComposeTester(() =>
        {
            for (var i = 0; i < 5; i++)
            {
                Button(() => { }, "Click");
            }
        });

        Assert.DoesNotThrow(() => composeTester.RecomposeAsync().Wait());
    }

    [Test]
    public void ButtonInsideDeepFor_Composing_NoException()
    {
        var composeTester = new ComposeTester.ComposeTester(() =>
        {
            for (var i = 0; i < 5; i++)
            {
                Box(content: () => { Button(() => { }, "Click"); });
            }
        });

        Assert.DoesNotThrow(() => composeTester.RecomposeAsync().Wait());
    }
}