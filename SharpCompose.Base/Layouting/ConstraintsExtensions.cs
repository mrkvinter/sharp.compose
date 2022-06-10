namespace SharpCompose.Base.Layouting;

public static class ConstraintsExtensions
{
    public static int ClampWidth(this Constraints constraints, int width) =>
        Math.Clamp(width, constraints.MinWidth, constraints.MaxWidth);

    public static int ClampHeight(this Constraints constraints, int height) =>
        Math.Clamp(height, constraints.MinHeight, constraints.MaxHeight);
}