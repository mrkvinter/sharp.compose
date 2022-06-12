using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SharpCompose.Base;
using SharpCompose.Base.Modifiers;
using TestSharpCompose.ComposeTester;
using TestSharpCompose.ComposeTester.Matchers;

namespace TestSharpCompose;

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

        await Task.Delay(100);
        var noData = composeTester.Root.OnNodeWithText("not data");
        var data = composeTester.Root.OnNodeWithText("1 2 3");

        Assert.IsNull(noData);
        Assert.IsNotNull(data);
    }

    [Test]
    public void LaunchedEffectNoScoped_CheckBeforeEndingEffect_StateNoChanged()
    {
        var composeTester = new ComposeTester.ComposeTester(FetchData_NoScopedState);

        var data = composeTester.Root.OnNodeWithText("not data");
        var noData = composeTester.Root.OnNodeWithText("1 2 3");

        Assert.IsNotNull(noData);
        Assert.IsNull(data);
    }

    [Test]
    public async Task LaunchedEffectNoScoped_CheckAfterEndingEffect_StateChanged()
    {
        var composeTester = new ComposeTester.ComposeTester(FetchData_NoScopedState);

        await Task.Delay(100);
        var data = composeTester.Root.OnNodeWithText("not data");
        var noData = composeTester.Root.OnNodeWithText("1 2 3");

        Assert.IsNull(noData);
        Assert.IsNotNull(data);
    }

    [Test]
    public async Task LaunchedEffect_NoScoped_NoAffectOtherState()
    {
        var composeTester = new ComposeTester.ComposeTester(FetchDataAndAnotherState_NoScopedState);

        composeTester.Root.OnNodeWithId("button")?.PerformClick();
        await Task.Delay(100);
        var anotherData = composeTester.Root.OnNodeWithId("anotherData")?.OnNodeWithText("2");

        Assert.IsNotNull(anotherData);
    }

    [Composable]
    private static void FetchData_ScopedState()
    {
        var data = Remember.Get(Array.Empty<int>);
        Remember.LaunchedEffect(async () => { data.Value = await GetData() ?? Array.Empty<int>(); });

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
        var data = Remember.Get(Array.Empty<int>);
        Remember.LaunchedEffect(async () => { data.Value = await GetData() ?? Array.Empty<int>(); });

        if (data.Value.Length == 0)
            Box(content: () => Text("not data")); //no_data
        else
            Box(content: () => Text(string.Join(" ", data.Value))); //no_data
    }

    [Composable]
    private static void FetchDataAndAnotherState_NoScopedState()
    {
        var anotherData = Remember.Get(0);
        var data = Remember.Get(Array.Empty<int>);
        Remember.LaunchedEffect(async () => { data.Value = await GetData() ?? Array.Empty<int>(); });

        Box(Modifier.With.Id("anotherData"), content: () => Text(anotherData.Value.ToString()));
        Button(() => anotherData.Value++, "+", Modifier.With.Id("button"));

        if (data.Value.Length == 0)
            Box(content: () => Text("not data"));
        else
            Box(content: () => Text(string.Join(" ", data.Value)));
    }
    
    private static async Task<int[]?> GetData()
    {
        await Task.Delay(100);

        return new[] {1, 2, 3};
    }
}