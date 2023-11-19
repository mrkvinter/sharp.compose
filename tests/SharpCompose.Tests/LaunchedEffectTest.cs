using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SharpCompose.Base;
using SharpCompose.Base.Extensions;
using SharpCompose.Base.Modifiers.Extensions;
using TestSharpCompose.ComposeTester;

namespace TestSharpCompose;

[NonParallelizable]
public class LaunchedEffectTest
{
    [Test]
    public void LaunchedEffectScoped_CheckBeforeEndingEffect_StateNoChanged()
    {
        var composeTester = new ComposeTester.ComposeTester(FetchData_ScopedState);

        var noData = composeTester.Root.OnNodeWithText("not data");
        var data = composeTester.Root.OnNodeWithText("1 2 3");

        Assert.IsNotNull(noData);
        Assert.IsNull(data);
    }

    [Test]
    public async Task LaunchedEffectScoped_CheckAfterEndingEffect_StateChanged()
    {
        var composeTester = new ComposeTester.ComposeTester(FetchData_ScopedState);

        await Task.Delay(200);
        await composeTester.RecomposeAsync();
        var noData = composeTester.Root.OnNodeWithText("not data");
        var data = composeTester.Root.OnNodeWithText("1 2 3");

        Assert.IsNull(noData);
        Assert.IsNotNull(data);
    }

    [Test]
    public void LaunchedEffectNoScoped_CheckBeforeEndingEffect_StateNoChanged()
    {
        var composeTester = new ComposeTester.ComposeTester(FetchData_NoScopedState);

        var noData = composeTester.Root.OnNodeWithText("not data");
        var data = composeTester.Root.OnNodeWithText("1 2 3");

        Assert.IsNotNull(noData);
        Assert.IsNull(data);
    }

    [Test]
    public async Task LaunchedEffectNoScoped_CheckAfterEndingEffect_StateChanged()
    {
        var composeTester = new ComposeTester.ComposeTester(FetchData_NoScopedState);

        await Task.Delay(200);
        await composeTester.RecomposeAsync();
        var noData = composeTester.Root.OnNodeWithText("not data");
        var data = composeTester.Root.OnNodeWithText("1 2 3");

        Assert.IsNull(noData);
        Assert.IsNotNull(data);
    }

    [Test]
    public async Task LaunchedEffect_NoScoped_NoAffectOtherState()
    {
        var composeTester = new ComposeTester.ComposeTester(FetchDataAndAnotherState_NoScopedState);

        composeTester.Root.OnNodeWithId("button")?.PerformClick();
        await composeTester.RecomposeAsync();
        var anotherData = composeTester.Root.OnNodeWithId("anotherData")?.OnNodeWithText("1");

        Assert.IsNotNull(anotherData);
    }
    
    [Test]
    public async Task LaunchedEffect_NoScoped_Dispose()
    {
        var box1Clicked = false.AsMutableState();
        var box2Clicked = false.AsMutableState();
        var composeTester = new ComposeTester.ComposeTester(() => DisappearingButton(box1Clicked, box2Clicked));

        composeTester.Root.OnNodeWithId("button")?.PerformClick();
        await composeTester.RecomposeAsync();
        composeTester.Root.OnNodeWithId("Box_1")?.PerformClick();
        await composeTester.RecomposeAsync();
        composeTester.Root.OnNodeWithId("Box_2")?.PerformClick();
        await composeTester.RecomposeAsync();

        Assert.IsFalse(box1Clicked.Value);
        Assert.IsTrue(box2Clicked.Value);
    }
    
    [Test]
    public async Task LaunchedEffect_BoxWithConstraints_Dispose()
    {
        var box1Clicked = false.AsMutableState();
        var box2Clicked = false.AsMutableState();
        var composeTester = new ComposeTester.ComposeTester(() => DisappearingButtonBoxWithConstraints(box1Clicked, box2Clicked));

        await composeTester.RecomposeAsync();
        composeTester.Size = new(50, 50);
        await composeTester.RecomposeAsync();
        composeTester.Root.OnNodeWithId("Box_1")?.PerformClick();
        composeTester.Root.OnNodeWithId("Box_2")?.PerformClick();
        await composeTester.RecomposeAsync();

        Assert.IsFalse(box1Clicked.Value);
        Assert.IsTrue(box2Clicked.Value);
    }
    
    [Test]
    public async Task LaunchedEffect_NoScopedAndNoUsed_Dispose()
    {
        var disposed = false.AsMutableState();
        var composeTester = new ComposeTester.ComposeTester(() => NotUsedLaunchedEffect(disposed));

        composeTester.Root.OnNodeWithId("button")?.PerformClick();
        await composeTester.RecomposeAsync();

        Assert.IsTrue(disposed.Value);
    }

    [Composable]
    private static void FetchData_ScopedState()
    {
        var data = Remember.Get(() => Array.Empty<int>().AsMutableState());
        Remember.LaunchedEffect(true, async _ => { data.Value = await GetData() ?? Array.Empty<int>(); });

        Box(content: () =>
        {
            if (data.Value.Length == 0)
                Box(content: () => Text("not data")); //no_data
            else
                Box(content: () => Text(string.Join(" ", data.Value))); //no_data
        });
    }

    [Composable]
    private static void FetchData_NoScopedState()
    {
        var data = Remember.Get(() => Array.Empty<int>().AsMutableState());
        Remember.LaunchedEffect(true, async _ => { data.Value = await GetData() ?? Array.Empty<int>(); });

        if (data.Value.Length == 0)
            Box(content: () => Text("not data")); //no_data
        else
            Box(content: () => Text(string.Join(" ", data.Value))); //data
    }

    [Composable]
    private static void FetchDataAndAnotherState_NoScopedState()
    {
        var anotherData = Remember.Get(() => 0.AsMutableState());
        var data = Remember.Get(() => Array.Empty<int>().AsMutableState());
        Remember.LaunchedEffect(true, async _ => { data.Value = await GetData() ?? Array.Empty<int>(); });

        Box(Modifier.Id("anotherData"), content: () => Text(anotherData.Value.ToString()));
        Button(() => anotherData.Value++, "+", Modifier.Id("button"));

        if (data.Value.Length == 0)
            Box(content: () => Text("not data"));
        else
            Box(content: () => Text(string.Join(" ", data.Value)));
    }
    
    [Composable]
    private static void DisappearingButton(MutableState<bool> box1Clicked, MutableState<bool> box2Clicked)
    {
        var clicked = Remember.Get(() => false.AsMutableState());

        Button(() => clicked.Value = true, "click", Modifier.Id("button"));

        if (!clicked.Value)
            Box(Modifier.Id("Box_1").Padding(50).Size(10).Clickable(() => box1Clicked.Value = true), content: () => Text("box 1"));
        else
            Box(Modifier.Id("Box_2").Padding(50).Size(10).Clickable(() => box2Clicked.Value = true), content: () => Text("box 2"));
    }
    
    [Composable]
    private static void DisappearingButtonBoxWithConstraints(MutableState<bool> box1Clicked, MutableState<bool> box2Clicked)
    {
        BoxWithConstraints(content: constraints =>
        {
            if (constraints.MaxWidth >= 100)
                Box(Modifier.Id("Box_1").Size(10).Clickable(() => box1Clicked.Value = true), content: () => Text("box 1"));
            else
                Box(Modifier.Id("Box_2").Size(10).Clickable(() => box2Clicked.Value = true), content: () => Text("box 2"));
        });
    }
    
    [Composable]
    private static void NotUsedLaunchedEffect(MutableState<bool> disposed)
    {
        var clicked = Remember.Get(() => false.AsMutableState());

        Button(() => clicked.Value = true, "click", Modifier.Id("button"));

        if (!clicked.Value)
            Remember.LaunchedEffect(true, () => {})
                .OnDispose(() => disposed.Value = true);
    }

    private static async Task<int[]?> GetData()
    {
        await Task.Delay(100);

        return new[] {1, 2, 3};
    }
}