using SharpCompose.Drawer.Core.Shapes;

namespace SharpCompose.Base.ComposesApi.Providers;

public class ShapeComponents
{
    public IShape Small { get; init; }
    public IShape Medium { get; init; }
    public IShape Large { get; init; }
}

public class LocalShapesProvider : LocalProvider<ShapeComponents>
{
    private static ShapeComponents DefaultValue => new()
    {
        Small = Shapes.Shapes.RoundCorner(4),
        Medium = Shapes.Shapes.RoundCorner(8),
        Large = Shapes.Shapes.RoundCorner(0),
    };

    static LocalShapesProvider()
    {
        Provide(DefaultValue).StartProvide();
    }
}