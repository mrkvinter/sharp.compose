namespace SharpCompose.Drawer.Core;

public class Outline
{
    private readonly List<Segment> segments = new();

    public IReadOnlyCollection<Segment> Segments => segments;

    private Outline()
    {
    }

    public static Outline StartShape(int x, int y)
    {
        var shape = new Outline();
        shape.segments.Clear();
        shape.segments.Add(new StartSegment(x, y));

        return shape;
    }

    public Outline LineTo(int x, int y)
    {
        segments.Add(new LineSegment(x, y));

        return this;
    }

    public Outline CubicBezierTo(int x, int y, int firstControlX, int firstControlY, int secondControlX, int secondControlY)
    {
        segments.Add(new CubicBezierSegment(x, y, firstControlX, firstControlY, secondControlX, secondControlY));

        return this;
    }

    public Outline Close()
    {
        segments.Add(new CloseSegment());

        return this;
    }
}