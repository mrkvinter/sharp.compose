using System;
using System.Drawing;
using System.Linq;
using SharpCompose.Base;
using SharpCompose.Base.Modifiers;
using SharpCompose.Base.Modifiers.Extensions;
using SharpCompose.Drawer.Core.Images;

namespace TestSharpCompose.TestComposables;


public static class WeatherCard
{
    public static void Card() =>
        Column(Modifier.With
                .Padding(15)
                .Clip(Shapes.RoundCorner(25))
                .BackgroundGradientColor("#FFCC7C".AsColor(), "#FFA954".AsColor(), (0.3f, 0), (0.85f, 1))
                .Shadow(Color.Black.WithAlpha(0.25f), (0, 4), 5, Shapes.RoundCorner(25))
                .Padding(24),
            content: () =>
            {
                Row(Modifier.With, Alignment.Center, content: () =>
                {
                    Text("+17°", 36, Color.White);
                    Spacer(Modifier.With.Width(7));
                    Icons.Thermometer(minHeight: 40);
                });
                Spacer(Modifier.With.Height(24));
                Row(verticalAlignment: Alignment.Center, content: () =>
                {
                    Icons.Sky(minHeight: 30);
                    Spacer(Modifier.With.Width(5));
                    Text("Cloudy", 24, Color.White);

                    Spacer(Modifier.With.Width(24));

                    Icons.Drop(minHeight: 30);
                    Spacer(Modifier.With.Width(5));
                    Text("71%", 24, Color.White);
                });
                Spacer(Modifier.With.Height(24));
                Row(verticalAlignment: Alignment.Center, content: () =>
                {
                    var startedHours = 18;

                    For(Enumerable.Range(0, 5), i =>
                    {
                        WeatherHour(startedHours + i, 1 - i / 2);
                        if (i != 4) Spacer(Modifier.With.Width(40));
                    });
                });
            });

    private static void WeatherHour(int hours, int temperature)
        => Column(horizontalAlignment: Alignment.Center, content: () =>
        {
            var sign = temperature > 0 ? "+" : temperature < 0 ? "-" : "";

            Text($"{hours}:00", 18, Color.White);
            Spacer(Modifier.With.Height(5));
            Icons.Sky(minWidth: 30);
            Spacer(Modifier.With.Height(5));
            Text($"{sign}{Math.Abs(temperature)}°", 18, Color.White);
        });

    private static class Icons
    {
        public static void Sky(int minWidth = 0, int minHeight = 0) => FakeIcon(minWidth, minHeight, "sky");
        public static void Drop(int minWidth = 0, int minHeight = 0) => FakeIcon(minWidth, minHeight, "drop");
        public static void Thermometer(int minWidth = 0, int minHeight = 0) => FakeIcon(minWidth, minHeight, "thermometer");
        
        private static void FakeIcon(int minWidth, int minHeight, string name)
        {
            Box(Modifier.With.BackgroundColor("#000".AsColor()));
            // var image = Remember.Get(() => Resource.Instance.GetResource<IImage>($"{name}-icon"));
            // Icon(image.Value,
            //     Modifier.With
            //         .MinSize(minWidth, minHeight));
        }
    }
}