using NUnit.Framework;
using SharpCompose.Base;
using TestSharpCompose.ComposeTester;
using TestSharpCompose.TestComposables;

namespace TestSharpCompose;

public sealed class LayoutTest
{
    [Test]
    public void LaunchedEffectScoped_CheckBeforeEndingEffect_StateNoChanged()
    {
        var composeTester = new ComposeTester.ComposeTester(WeatherCard.Card);

        Composer.Layout();
        
        // var noData = composeTester.Root.OnNodeWithText("not data");
        // var data = composeTester.Root.OnNodeWithText("1 2 3");
        //
        // Assert.IsNotNull(noData);
        // Assert.IsNull(data);
    }

}