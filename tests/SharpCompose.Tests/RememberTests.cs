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

    private static void RememberContent()
    {
        var state = Remember.Get(() => false.AsMutableState());
        var v = Remember.Get(state.Value, () => new TestValue());

        Button(() => state.Value = true, label: "Click", Modifier.Id("State"));
        Button(() => v.Value++, label: "Increment", Modifier.Id("Increment"));

        Text(v.Value.ToString());
    }
}