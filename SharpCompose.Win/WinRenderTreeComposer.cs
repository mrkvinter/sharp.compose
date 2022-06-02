using System;
using System.Collections.Generic;
using SharpCompose.Base;
using SharpCompose.Drawer.Core;

namespace SharpCompose.Win;

public class WinRenderTreeComposer : Composer
{
    public override void BuildAttributes(IReadOnlyDictionary<string, object> attributes)
    {
        throw new NotImplementedException();
    }

    public void Build(ICanvas canvas)
    {
        foreach (var scope in Root.Child)
            scope.ElementBuilder.Draw(scope, canvas);
    }
}