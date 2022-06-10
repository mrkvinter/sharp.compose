﻿using System.Drawing;
using SharpCompose.Drawer.Core;

namespace SharpCompose.Base.ComposesApi.Providers;

public class TextStyle
{
    public Color? Color { get; init; } = null;
    public int FontSize { get; init; } = 14;
    public Font Font { get; init; } = new Font("Helvetica", FontWeight.Regular);
}

public class LocalTextStyleProvider : LocalProvider<TextStyle>
{
    private static TextStyle DefaultValue = new();

    static LocalTextStyleProvider()
    {
        Provide(DefaultValue).StartProvide();
    }
}