using System.Threading.Tasks;
using NUnit.Framework;
using SharpCompose.Base;
using SharpCompose.Base.Extensions;
using TestSharpCompose.ComposeTester;

namespace TestSharpCompose;

public class RememberTests
{
    private class TestValue
    {
        public int Value { get; set; }
    }

    [Test]
    public void Remember_KeyChanged_PropertyValueOrigin()
    {
        var composeTester = new ComposeTester.ComposeTester(RememberContent);

        var node = composeTester.Root.OnNodeWithText("0");

        Assert.IsNotNull(node);
    }

    [Test]
    public async Task Remember_ClickForIncrementAndNoChangeKey_PropertyValueChanged()
    {
        var composeTester = new ComposeTester.ComposeTester(RememberContent);

        composeTester.Root.OnNodeWithId("Increment")!.PerformClick();
        await composeTester.RecomposeAsync();
        var node = composeTester.Root.OnNodeWithText("1");

        Assert.IsNotNull(node);
    }

    [Test]
    public async Task Remember_TwiceClickForIncrementAndNoChangeKey_PropertyValueChangedTwice()
    {
        var composeTester = new ComposeTester.ComposeTester(RememberContent);

        composeTester.Root.OnNodeWithId("Increment")!.PerformClick();
        await composeTester.RecomposeAsync();
        composeTester.Root.OnNodeWithId("Increment")!.PerformClick();
        await composeTester.RecomposeAsync();
        var node = composeTester.Root.OnNodeWithText("2");

        Assert.IsNotNull(node);
    }

    [Test]
    public async Task Remember_ClickForIncrementAndChangeKey_PropertyValueOrigin()
    {
        var composeTester = new ComposeTester.ComposeTester(RememberContent);

        composeTester.Root.OnNodeWithId("Increment")!.PerformClick();
        await composeTester.RecomposeAsync();
        composeTester.Root.OnNodeWithId("State")!.PerformClick();
        await composeTester.RecomposeAsync();
        var node = composeTester.Root.OnNodeWithText("0");

        Assert.IsNotNull(node);
    }

    [Test]
    public async Task RememberPage_OriginState_ZeroPageOpened()
    {
        var composeTester = new ComposeTester.ComposeTester(RouterContent);

        await composeTester.RecomposeAsync();
        var page0Line1 = composeTester.Root.OnNodeWithText("Page 0 - line 1");
        var page0Line2 = composeTester.Root.OnNodeWithText("Page 0 - line 2");
        var page1 = composeTester.Root.OnNodeWithText("Page 1 - line 1");

        Assert.That(page0Line1, Is.Not.Null);
        Assert.That(page0Line2, Is.Not.Null);
        Assert.That(page1, Is.Null);
    }

    [Test]
    public async Task RememberPage_OpenPage_OnlyOnePageVisible()
    {
        var composeTester = new ComposeTester.ComposeTester(RouterContent);

        composeTester.Root.OnNodeWithId("Button Page 1")!.PerformClick();
        await composeTester.RecomposeAsync();
        var page0Line1 = composeTester.Root.OnNodeWithText("Page 0 - line 1");
        var page0Line2 = composeTester.Root.OnNodeWithText("Page 0 - line 2");
        var page1 = composeTester.Root.OnNodeWithText("Page 1 - line 1");

        Assert.That(page0Line1, Is.Null);
        Assert.That(page0Line2, Is.Null);
        Assert.That(page1, Is.Not.Null);
    }

    [Composable]
    private static void RememberContent()
    {
        var state = Remember.Get(() => false.AsMutableState());
        var v = Remember.Get(state.Value, () => new TestValue());

        Button(() => state.Value = true, label: "Click", Modifier.Id("State"));
        Button(() => v.Value++, label: "Increment", Modifier.Id("Increment"));

        Text(v.Value.ToString());
    }

    [Composable]
    private static void RouterContent()
    {
        var page = Remember.Get(() => 0.AsMutableState());

        Button(() => page.Value = 0, label: "Button Page 0", Modifier.Id("Button Page 0"));
        Button(() => page.Value = 1, label: "Button Page 1", Modifier.Id("Button Page 1"));

        Box(content: () =>
            Box(alignment: Alignment.TopStart, content: page.Value switch
            {
                0 => Page0,
                1 => Page1,
                _ => () => { }
            }));
    }

    [Composable]
    private static void Page0() => Box(content: () =>
    {
        Text("Page 0 - line 1");
        Text("Page 0 - line 2");
    });

    [Composable]
    private static void Page1() => Box(content: () =>
    {
        Text("Page 1 - line 1");
    });
}