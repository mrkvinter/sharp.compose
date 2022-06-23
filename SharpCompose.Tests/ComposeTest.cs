using System.Diagnostics;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using SharpCompose.Base;
using SharpCompose.WebTags;
using TestSharpCompose.TestComposables;
using TestSharpCompose.TestComposer;

namespace TestSharpCompose;

public class ComposeTest
{
    
    [Test]
    public void StateCompose_ButtonClick_CorrectNewState()
    {
        var inputHandler = A.Fake<IInputHandler>();
        var treeBuilder = new TestTreeBuilder();
        TreeBuilder.Instance = treeBuilder;

        Composer.Compose(inputHandler, TestWebController.WithStateCompose);
        Composer.Layout();
        
        treeBuilder.Root.GetById("button")?.OnClick?.Invoke();
        
        Composer.Compose(inputHandler, TestWebController.WithStateCompose);
        Composer.Layout();

        var content = treeBuilder.Root.GetById("result")?.Content;

        Assert.That(content, Is.EqualTo("1"));
    }

    [Test]
    public void StateCompose_ButtonClick_CorrectNewFlagState()
    {
        var inputHandler = A.Fake<IInputHandler>();
        var treeBuilder = new TestTreeBuilder();
        TreeBuilder.Instance = treeBuilder;

        Composer.Compose(inputHandler, TestWebController.WithSeveralStateCompose);
        Composer.Layout();
        treeBuilder.Root.GetById("button")?.OnClick?.Invoke();
        Composer.Compose(inputHandler, TestWebController.WithSeveralStateCompose);
        Composer.Layout();

        var content = treeBuilder.Root.GetById("result")?.Content;
        var showedTag = treeBuilder.Root.GetById("no-hidden");
        var hiddenTag = treeBuilder.Root.GetById("hidden");

        Assert.That(content, Is.EqualTo("1"));
        Assert.That(showedTag, Is.Not.Null);
        Assert.That(hiddenTag, Is.Null);
    }

    [Test]
    public async Task LaunchedEffect_ScopedState_StateChanged()
    {
        var inputHandler = A.Fake<IInputHandler>();
        var treeBuilder = new TestTreeBuilder();

        Composer.Compose(inputHandler, TestWebController.FetchData_ScopedState);
        await Task.Delay(120);
        Composer.Compose(inputHandler, TestWebController.FetchData_ScopedState);
        var data = treeBuilder.Root.GetById("data");
        var noData = treeBuilder.Root.GetById("no_data");

        Assert.IsNull(noData);
        Assert.IsNotNull(data);
    }

    [Test, Ignore("Not implemented")]
    public async Task LaunchedEffect_NoScopedState_StateChanged()
    {
        var inputHandler = A.Fake<IInputHandler>();
        var treeBuilder = new TestTreeBuilder();

        Composer.Compose(inputHandler, TestWebController.FetchData_NoScopedState);
        await Task.Delay(120);
        Composer.Compose(inputHandler, TestWebController.FetchData_NoScopedState);
        var data = treeBuilder.Root.GetById("data");
        var noData = treeBuilder.Root.GetById("no_data");

        Assert.IsNull(noData);
        Assert.IsNotNull(data);
    }

    [Test]
    public void TestComplex()
    {
        static void Compose() => Composer.Compose(A.Fake<IInputHandler>(), TestWebController.ComplexCompose);
        static void Recompose() => Composer.Compose(A.Fake<IInputHandler>(), TestWebController.ComplexCompose);

        var sw = new Stopwatch();

        sw.Start();
        Compose();
        Composer.Layout();
        sw.Stop();
        var resultCompose = sw.ElapsedMilliseconds;
        
        sw.Restart();
        Recompose();
        Composer.Layout();
        sw.Stop();
        var resultRecompose = sw.ElapsedMilliseconds;

        Assert.That(resultCompose, Is.LessThan(500));
        Assert.That(resultRecompose, Is.LessThan(150));
    }
}
