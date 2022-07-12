using NUnit.Framework;
using TestSharpCompose.ComposeTester;

namespace TestSharpCompose;

public class LayoutWithLoopTests
{
    [Test]
    public void ForLoop_TextsWithCounter_AllTextsDisplayed()
    {
        var composeTester = new ComposeTester.ComposeTester(() =>
        {
            for (var i = 0; i < 5; i++)
                Text(i.ToString());
        });

        var node0 = composeTester.Root.OnNodeWithText("0");
        var node1 = composeTester.Root.OnNodeWithText("1");
        var node2 = composeTester.Root.OnNodeWithText("2");
        var node3 = composeTester.Root.OnNodeWithText("3");

        Assert.IsNotNull(node0);
        Assert.IsNotNull(node1);
        Assert.IsNotNull(node2);
        Assert.IsNotNull(node3);
    }

    [Test]
    public void ContentBeforeForLoop_TextsWithCounter_AllTextsDisplayed()
    {
        var composeTester = new ComposeTester.ComposeTester(() =>
        {
            Text("ContentBefore");
            for (var i = 0; i < 5; i++)
                Text(i.ToString());
        });

        var nodeContent = composeTester.Root.OnNodeWithText("ContentBefore");
        var node0 = composeTester.Root.OnNodeWithText("0");
        var node1 = composeTester.Root.OnNodeWithText("1");
        var node2 = composeTester.Root.OnNodeWithText("2");
        var node3 = composeTester.Root.OnNodeWithText("3");

        Assert.IsNotNull(nodeContent);
        Assert.IsNotNull(node0);
        Assert.IsNotNull(node1);
        Assert.IsNotNull(node2);
        Assert.IsNotNull(node3);
    }

    [Test]
    public void ContentAfterForLoop_TextsWithCounter_AllTextsDisplayed()
    {
        var composeTester = new ComposeTester.ComposeTester(() =>
        {
            for (var i = 0; i < 5; i++)
                Text(i.ToString());

            Text("ContentAfter");
        });

        var nodeContent = composeTester.Root.OnNodeWithText("ContentAfter");
        var node0 = composeTester.Root.OnNodeWithText("0");
        var node1 = composeTester.Root.OnNodeWithText("1");
        var node2 = composeTester.Root.OnNodeWithText("2");
        var node3 = composeTester.Root.OnNodeWithText("3");

        Assert.IsNotNull(nodeContent);
        Assert.IsNotNull(node0);
        Assert.IsNotNull(node1);
        Assert.IsNotNull(node2);
        Assert.IsNotNull(node3);
    }
}