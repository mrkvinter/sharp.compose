using SharpCompose.Base.Layouting;
using SharpCompose.Base.Modifiers.LayoutModifiers;
using SharpCompose.Drawer.Core;
using SharpCompose.Drawer.Core.Images;

namespace SharpCompose.Base.Modifiers.DrawableModifiers;

public sealed class ImageDrawingModifier : BaseDrawableModifier
{
    private readonly IImage image;
    public ImageDrawingModifier(IImage image)
    {
        this.image = image;
    }

    protected override void Draw(IGraphics graphics, (int w, int h) size, (int x, int y) offset)
        => graphics.DrawImage(offset, size, image);
}

public sealed class ImageMeasureModifier : ILayoutModifier
{
    private readonly IImage image;
    public ImageMeasureModifier(IImage image)
    {
        this.image = image;
    }

    public MeasureResult Measure(Measurable measurable, Constraints constraints)
    {
        var childConstraints = Constraints.MinSize(image.Width, image.Height);
        childConstraints = constraints.Constraint(childConstraints);
        var measureResult = measurable.Measure(childConstraints);
        var scaleFactor = Math.Min((float)measureResult.Width / image.Width, (float)measureResult.Height / image.Height);

        return measureResult with
        {
            Width = (int)Math.Round(image.Width * scaleFactor),
            Height = (int)Math.Round(image.Height * scaleFactor)
        };
    }
}