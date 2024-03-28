using System;
using System.Threading.Tasks;
using FakeItEasy;
using SharpCompose.Base;
using SharpCompose.Base.Modifiers.Extensions;
using SharpCompose.Base.Nodes;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Utilities;

namespace TestSharpCompose.ComposeTester;

public class ComposeTester
{
    private IntSize size = new(800, 600);
    private readonly InputHandler inputHandler;
    public IntSize Size
    {
        get => size;
        set
        {
            size = value;
            Composer.Recompose();
        }
    }

    public ComposeTester(Action setContent)
    {
        var canvas = A.Fake<ICanvas>();
        inputHandler = new InputHandler(SetCursor);
        A.CallTo(() => canvas.Size).ReturnsLazily(() => Size);

        Composer.Instance.ForceClear();
        Composer.Instance.Init(canvas);
        Composer.Compose(inputHandler, setContent);
        Composer.Layout();
        Composer.Draw();
    }

    public async Task RecomposeAsync(bool forceRecompose = false)
    {
        while (Composer.Instance.RecomposingAsk || forceRecompose)
        {
            forceRecompose = false;
            Composer.RecomposeTree();
            Composer.Layout();

            await Task.Yield();
        }
    }

    public Node Root => new(Composer.Instance.Root, this);

    public void Click(BoundState mousePos)
    {
        inputHandler.OnMouseMove(mousePos.XOffset, mousePos.YOffset);
        inputHandler.OnMouseDown();
        inputHandler.OnMouseUp();
    }

    public void SetCursor(Cursor cursor)
    {
    }
}

public record Node(LayoutNode LayoutNode, ComposeTester ComposeTester);