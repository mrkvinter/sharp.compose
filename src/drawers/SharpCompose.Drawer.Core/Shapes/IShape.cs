namespace SharpCompose.Drawer.Core.Shapes;

public interface IShape
{
    Outline CreateOutline(int width, int height);
}
