using System;
using FakeItEasy;
using SharpCompose.Base;
using SharpCompose.Drawer.Core;

namespace TestSharpCompose.ComposeTester;

public class ComposeTester
{
    private readonly IInputHandler inputHandler;
    private readonly ICanvas canvas;
    private readonly Action setContent;

    public ComposeTester(Action setContent)
    {
        this.setContent = setContent;
        inputHandler = A.Fake<IInputHandler>();
        canvas = A.Fake<ICanvas>();
        A.CallTo(() => canvas.Size).Returns((800, 600));

        Composer.Instance.Init(canvas);
        Composer.Compose(inputHandler, setContent);
    }

    public Node Root => new Node(Composer.Instance.Root);
}

public record Node(Composer.Scope Scope);