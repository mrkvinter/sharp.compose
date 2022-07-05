using System;
using System.Threading.Tasks;
using FakeItEasy;
using SharpCompose.Base;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Utilities;

namespace TestSharpCompose.ComposeTester;

public class ComposeTester
{
    private IntSize size = new(800, 600);

    public IntSize Size
    {
        get => size;
        set
        {
            size = value;
            Composer.Recompose();
        }
    }

    private readonly Action setContent;
    private readonly IInputHandler inputHandler;

    public ComposeTester(Action setContent)
    {
        this.setContent = setContent;
        inputHandler = A.Fake<IInputHandler>();
        var canvas = A.Fake<ICanvas>();
        A.CallTo(() => canvas.Size).ReturnsLazily(() => Size);

        Composer.Instance.ForceClear();
        Composer.Instance.Init(canvas);
        Composer.Compose(inputHandler, setContent);
        Composer.Layout();
    }

    public async Task RecomposeAsync(bool forceRecompose = false)
    {
        while (Composer.Instance.RecomposingAsk || forceRecompose)
        {
            forceRecompose = false;
            Composer.Compose(inputHandler, setContent);
            Composer.Layout();

            await Task.Yield();
        }
    }

    public Node Root => new(Composer.Instance.Root);
}

public record Node(LayoutNode LayoutNode);