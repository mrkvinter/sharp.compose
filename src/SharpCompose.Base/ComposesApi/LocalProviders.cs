using SharpCompose.Base.ComposesApi.Providers;
using SharpCompose.Drawer.Core;

namespace SharpCompose.Base.ComposesApi;

public static partial class BaseCompose
{
    public static readonly LocalProvider<Colors> LocalColors = new(new Colors
    (
        Accent: "#005FB8".AsColor(),
        OnAccent: "#FFFFFF".AsColor(),
        Standard: "#FFFFFF".AsColor(),
        OnStandard: "#000000".AsColor(),
        Background: "#F3F3F3".AsColor()
    ));

    public static readonly LocalProvider<float> LocalContentAlpha = new(1);

    public static readonly LocalProvider<IInputHandler> LocalInputHandler = new(default!);

    public static readonly LocalProvider<TextStyle> LocalTextStyle = new(new TextStyle(14, new Font("Helvetica", FontWeight.Regular)));

    public static readonly LocalProvider<ShapeComponents> LocalShape = new(new ShapeComponents
    {
        Small = Shapes.RoundCorner(4),
        Medium = Shapes.RoundCorner(8),
        Large = Shapes.RoundCorner(0),
    });
}

