namespace SharpCompose.Base.ComposesApi.Providers;

public static class LocalProviders
{
    public static LocalProvider<Colors> Colors = new LocalProvider<Colors>(new()
    {
        Accent = "#005FB8".AsColor(),
        OnAccent = "#FFFFFF".AsColor(),
        Standard = "#FFFFFF".AsColor(),
        OnStandard = "#000000".AsColor(),
        Background = "#F3F3F3".AsColor()
    });
    
    public static LocalProvider<float> Alpha = new(1);
    
    public static LocalProvider<IInputHandler?> InputHandler = new(default!);

    public static LocalProvider<TextStyle> TextStyle = new(new TextStyle());

    public static LocalProvider<ShapeComponents> Shape = new(new ShapeComponents
    {
        Small = Shapes.RoundCorner(4),
        Medium = Shapes.RoundCorner(8),
        Large = Shapes.RoundCorner(0),
    });
}