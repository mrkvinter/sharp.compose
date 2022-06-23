namespace SharpCompose.Drawer.Core;

public abstract record Segment;

public sealed record StartSegment(int X, int Y) : Segment;

public sealed record LineSegment(int X, int Y) : Segment;

public sealed record CubicBezierSegment(int X, int Y, int FirstControlX, int FirstControlY, int SecondControlX, int SecondControlY) : Segment;

public sealed record CloseSegment : Segment;