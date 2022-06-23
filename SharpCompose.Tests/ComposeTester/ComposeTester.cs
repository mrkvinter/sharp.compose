using System;
using FakeItEasy;
using SharpCompose.Base;
using SharpCompose.Drawer.Core;

namespace TestSharpCompose.ComposeTester;

public class ComposeTester
{
    public ComposeTester(Action setContent)
    {
        var inputHandler = A.Fake<IInputHandler>();
        var canvas = A.Fake<ICanvas>();
        A.CallTo(() => canvas.Size).Returns((800, 600));

        Composer.Instance.Init(canvas);
        Composer.Compose(inputHandler, setContent);
    }

    public Node Root => new Node(Composer.Instance.Root);
}

public record Node(Composer.Scope Scope);