using System.Diagnostics;
using Microsoft.AspNetCore.Components.Rendering;
using NUnit.Framework;
using SharpCompose.Base;
using SharpCompose.Base.Composers;

namespace TestSharpCompose;

public class ComposeTest
{
    [Test]
    public void Test1()
    {
        var composer = (RenderTreeComposer)Composer.Instance;
        
        Composer.RootComposer(TestController.SimpleCompose);

        var treeBuilder = new RenderTreeBuilder();
        composer.Build(treeBuilder);
    }

    [Test]
    public void TestComplex()
    {
        var treeBuilder = new RenderTreeBuilder();
        var composer = (RenderTreeComposer)Composer.Instance;
        var sw = new Stopwatch();

        sw.Start();
        TestController.ComplexCompose();
        composer.Build(treeBuilder);
        sw.Stop();

        var result = sw.ElapsedMilliseconds;
        Assert.That(result, Is.LessThan(100));
    }

    public struct Foo
    {
        public string? Test { get; set; }
        public string[]? Arr { get; init; }
    }
}