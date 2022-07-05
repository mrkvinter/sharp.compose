using System.Drawing;

namespace SharpCompose.Base.ComposesApi.Providers;

public record Colors(
    Color Accent,
    Color OnAccent,
    Color Standard,
    Color OnStandard,
    Color Background)
{
    public Colors Copy() => new(Accent, OnAccent, Standard, OnStandard, Background);
}