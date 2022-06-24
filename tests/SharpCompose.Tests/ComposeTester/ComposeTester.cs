using System;
using System.Threading.Tasks;
using FakeItEasy;
using SharpCompose.Base;
using SharpCompose.Drawer.Core;

namespace TestSharpCompose.ComposeTester;

public class ComposeTester
{
    private readonly Action setContent;
    private readonly IInputHandler inputHandler;

    public ComposeTester(Action setContent)
    {
        this.setContent = setContent;
        inputHandler = A.Fake<IInputHandler>();
        var canvas = A.Fake<ICanvas>();
        A.CallTo(() => canvas.Size).Returns((800, 600));

        Composer.Instance = new Composer();
        Composer.Instance.Init(canvas);
        Composer.Compose(inputHandler, setContent);
    }

    public async Task RecomposeAsync()
    {
        while (Composer.Instance.RecomposingAsk)
        {
            Composer.Compose(inputHandler, setContent);
            Composer.Layout();

            await Task.Yield();
        }
    }

    public Node Root => new Node(Composer.Instance.Root);
}

public record Node(Composer.Scope Scope);