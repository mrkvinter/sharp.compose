using System.Drawing;
using SharpCompose.Base.Shapes;

namespace SharpCompose.Base.ComposesApi.Providers;

public class Colors
{
    public Color Accent { get; init; }
    public Color OnAccent { get; init; }
    public Color Standard { get; init; }
    public Color OnStandard { get; init; }
    public Color Background { get; init; }
}

public class LocalColorsProvider : LocalProvider<Colors>
{
    private static Colors DefaultValue => new()
    {
        Accent = "#005FB8".AsColor(),
        OnAccent = "#FFFFFF".AsColor(),
        Standard = "#FFFFFF".AsColor(),
        OnStandard = "#000000".AsColor(),
        Background = "#F3F3F3".AsColor()
    };


    static LocalColorsProvider()
    {
        Provide(DefaultValue).StartProvide();
    }
}