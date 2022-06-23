using System;
using System.Drawing;
using System.Linq;
using SharpCompose.Base;
using SharpCompose.Base.Modifiers.Extensions;

namespace TestSharpCompose.TestComposables;


public static class WeatherCard
{
    public static void Card() =>
        Column(Modifier
                .Padding(15)
                .Clip(Shapes.RoundCorner(25))
                .BackgroundGradientColor("#FFCC7C".AsColor(), "#FFA954".AsColor(), (0.3f, 0), (0.85f, 1))
                .Shadow(Color.Black.WithAlpha(0.25f), (0, 4), 5, Shapes.RoundCorner(25))
                .Padding(24),
            content: () =>
            {
                Row(verticalAlignment: Alignment.Center, content: () =>
                {
                    Text("+17°", 36, Color.White);
                    Spacer(Modifier.Width(7));
                    Icons.Thermometer();
                });
                Spacer(Modifier.Height(24));
                Row(verticalAlignment: Alignment.Center, content: () =>
                {
                    Icons.Sky();
                    Spacer(Modifier.Width(5));
                    Text("Cloudy", 24, Color.White);

                    Spacer(Modifier.Width(24));

                    Icons.Drop();
                    Spacer(Modifier.Width(5));
                    Text("71%", 24, Color.White);
                });
                Spacer(Modifier.Height(24));
                Row(verticalAlignment: Alignment.Center, content: () =>
                {
                    var startedHours = 18;

                    For(Enumerable.Range(0, 5), i =>
                    {
                        WeatherHour(startedHours + i, 1 - i / 2);
                        if (i != 4) Spacer(Modifier.Width(40));
                    });
                });
            });

    private static void WeatherHour(int hours, int temperature)
        => Column(horizontalAlignment: Alignment.Center, content: () =>
        {
            var sign = temperature > 0 ? "+" : temperature < 0 ? "-" : "";

            Text($"{hours}:00", 18, Color.White);
            Spacer(Modifier.Height(5));
            Icons.Sky();
            Spacer(Modifier.Height(5));
            Text($"{sign}{Math.Abs(temperature)}°", 18, Color.White);
        });

    private static class Icons
    {
        public static void Sky() => FakeIcon();
        public static void Drop() => FakeIcon();
        public static void Thermometer() => FakeIcon();

        private static void FakeIcon() => Box(Modifier.BackgroundColor("#000".AsColor()));
    }
}