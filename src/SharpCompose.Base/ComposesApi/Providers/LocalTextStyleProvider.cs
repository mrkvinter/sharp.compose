using System.Drawing;
using SharpCompose.Drawer.Core;

namespace SharpCompose.Base.ComposesApi.Providers;

public record TextStyle(double FontSize, Font Font)
{
    public Color? Color { get; init; }
}
