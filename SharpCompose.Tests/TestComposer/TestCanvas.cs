using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Brushes;
using SharpCompose.Drawer.Core.Images;
using SharpCompose.Drawer.Core.Shapes;

namespace TestSharpCompose.TestComposer;

public class TestCanvas : ICanvas
{
    public void Draw()
    {
    }

    public (int w, int h) MeasureText(string text, double emSize, Font font) => (0, 0);

    public IGraphics StartGraphics() => new TestGraphics();

    public void DrawGraphics(int x, int y, IGraphics graphics)
    {
    }

    public void Clear()
    {
    }

    public (int w, int h) Size { get; set; }

    public class TestGraphics : IGraphics
    {
        public void FillRectangle((int x, int y) point, (int w, int h) size, Brush brush)
        {
        }

        public void StrokeShape((int x, int y) point, IShape shape, int width, int height, int lineWidth, Brush brush)
        {
        }

        public void DrawGraphics(IGraphics otherGraphics, int x, int y)
        {
        }

        public void DrawText(string text, double emSize, Font font, Brush brush, int x, int y) =>
            TestTreeBuilder.Current.Content = text;

        public void DrawShadow((int x, int y) point, (int w, int h) size, (int x, int y) offset, int blurRadius,
            IShape shape,
            Brush brush)
        {
        }

        public void DrawImage((int x, int y) point, (int w, int h) size, IImage image)
        {
            
        }

        public void Clip(IShape shape, (int x, int y) offset, (int w, int h) size)
        {
        }

        public void Clear()
        {
        }
    }
}