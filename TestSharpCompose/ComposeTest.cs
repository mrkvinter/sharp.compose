using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using SharpCompose.Base;
using TestSharpCompose.TestComposer;

namespace TestSharpCompose;

public class ComposeTest
{
    [Test]
    public void TestStateCompose()
    {
        var composer = new TestComposer.TestComposer();
        Composer.Instance = composer;

        Composer.RootComposer(TestController.WithStateCompose);
        composer.Build().GetById("button")?.OnClick?.Invoke();
        Composer.RootComposer(TestController.WithStateCompose);
        var content = composer.Build().GetById("result")?.Child[0].Content;

        Assert.That(content, Is.EqualTo("1"));
    }

    [Test]
    public async Task LaunchedEffect_ScopedState_StateChanged()
    {
        var composer = new TestComposer.TestComposer();
        Composer.Instance = composer;

        Composer.RootComposer(TestController.FetchData_ScopedState);
        await Task.Delay(120);
        Composer.RootComposer(TestController.FetchData_ScopedState);
        var root = composer.Build();
        var data = root.GetById("data");
        var noData = root.GetById("no_data");

        Assert.IsNull(noData);
        Assert.IsNotNull(data);
    }

    [Test]
    public async Task LaunchedEffect_NoScopedState_StateChanged()
    {
        var composer = new TestComposer.TestComposer();
        Composer.Instance = composer;

        Composer.RootComposer(TestController.FetchData_NoScopedState);
        await Task.Delay(120);
        Composer.RootComposer(TestController.FetchData_NoScopedState);
        var root = composer.Build();
        var data = root.GetById("data");
        var noData = root.GetById("no_data");

        Assert.IsNull(noData);
        Assert.IsNotNull(data);
    }

    [Test]
    public void TestComplex()
    {
        static void Compose() => Composer.RootComposer(TestController.ComplexCompose);
        static void Recompose() => Composer.RootComposer(TestController.ComplexCompose);
        
        var composer = new TestComposer.TestComposer();
        Composer.Instance = composer;
        var sw = new Stopwatch();

        sw.Start();
        Compose();
        composer.Build();
        sw.Stop();
        var resultCompose = sw.ElapsedMilliseconds;
        
        sw.Restart();
        Recompose();
        composer.Build();
        sw.Stop();
        var resultRecompose = sw.ElapsedMilliseconds;

        Assert.That(resultCompose, Is.LessThan(500));
        Assert.That(resultRecompose, Is.LessThan(100));
    }
}