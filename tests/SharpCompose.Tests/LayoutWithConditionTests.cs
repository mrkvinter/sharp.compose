using System.Threading.Tasks;
using NUnit.Framework;
using SharpCompose.Base;
using SharpCompose.Base.Extensions;
using TestSharpCompose.ComposeTester;

namespace TestSharpCompose;

public class LayoutWithConditionTests
{
    [Test]
    public async Task ButtonWithCondition_Clicked_TextChanged()
    {
        var composeTester = new ComposeTester.ComposeTester(() =>
        {
            var state = Remember.Get(() => true.AsMutableState());
            Button(() => state.Value = !state.Value, "Toggle");
            if (state.Value)
            {
                Button(() => { }, "True");
            }
        });

        composeTester.Root.OnNodeWithText("Toggle")!.PerformClick();
        await composeTester.RecomposeAsync();
        composeTester.Root.OnNodeWithText("Toggle")!.PerformClick();
        await composeTester.RecomposeAsync();
        composeTester.Root.OnNodeWithText("True");
        
        Assert.IsNotNull(composeTester.Root.OnNodeWithText("True"));
    }

    [Test]
    public async Task TestApp()
    {
        var composeTester = new ComposeTester.ComposeTester(() =>
        {
            var state = Remember.Get(() => true.AsMutableState());
            Button(() => state.Value = !state.Value, "Toggle");
            if (state.Value)
            {
                Button(() => { }, "True");
            }
        });

        composeTester.Root.OnNodeWithText("Toggle")!.PerformClick();
        await composeTester.RecomposeAsync();
        composeTester.Root.OnNodeWithText("True");
        
        Assert.IsNull(composeTester.Root.OnNodeWithText("True"));
    }
}