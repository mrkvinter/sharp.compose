using System;
using System.Collections.Generic;
using SharpCompose.Base;
using SharpCompose.Drawer.Core;

namespace SharpCompose.Win;

public class WinRenderTreeComposer : Composer
{
    // public override void BuildAttributes(IReadOnlyDictionary<string, object> attributes)
    // {
    //     throw new NotImplementedException();
    // }

    // public void Build(ICanvas canvas)
    // {
    //     TextElementBuilder.MeasureText = (text, size, font) => canvas.MeasureText(text, size, font);
    //     Canvas = canvas;
    //
    //     var measure = Root.GetMeasure().Invoke(new Constraints());
    //     measure.Placeable(0, 0);
    //     //
    //     // foreach (var scope in Root.Children)
    //     // {
    //     //     scope.ElementBuilder.Draw(scope, canvas);
    //     // }
    // }

    // private void Layout(Scope scope, ICanvas canvas)
    // {
    //     MeasureChildren();
    //     MeasureOwnSize();
    //     PlaceChildren();
    // }
    //
    // private void Draw(Scope scope, ICanvas canvas)
    // {
    //     DrawSelf();
    //     DrawChildren();
    // }
}
