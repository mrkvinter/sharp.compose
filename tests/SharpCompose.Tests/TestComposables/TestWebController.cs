using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCompose.Base;
using static SharpCompose.Base.ComposesApi.BaseCompose;
using static SharpCompose.WebTags.HtmlCompose;

namespace TestSharpCompose.TestComposables;

public static class TestWebController
{
    [Composable]
    public static void SimpleCompose() =>
        Div(attr =>
        {
            attr.Id("root");
            attr.Class("text-center");
        }, () =>
        {
            H1(child: () => Text("Header"));
            P(child: () => Text("Article"));
        });

    [Composable]
    public static void WithStateCompose() =>
        Div(attr =>
        {
            attr.Id("root");
            attr.Class("text-center");
        }, () =>
        {
            var counter = Remember.Get(0);

            H1(child: () => Text("Header"));
            P(atr => atr.Id("result"), () => Text(counter.Value.ToString()));
            Button(() => counter.Value++, atr => atr.Id("button"));
        });

    [Composable]
    public static void WithSeveralStateCompose() =>
        Div(attr =>
        {
            attr.Id("root");
            attr.Class("text-center");
        }, () =>
        {
            var counter = Remember.Get(0);
            var isOpen = Remember.Get(false);

            H1(child: () => Text("Header"));
            P(atr => atr.Id("result"), () => Text(counter.Value.ToString()));
            Button(() =>
            {
                counter.Value++;
                isOpen.Value = true;
            }, atr => atr.Id("button"));
            
            if (isOpen.Value)
                P(atr => atr.Id("no-hidden"));
            else
                P(atr => atr.Id("hidden"));
        });

    [Composable]
    public static void ComplexCompose() =>
        Div(attr =>
        {
            attr.Id("root");
            attr.Class("text-center");
        }, () =>
        {
            H1(child: () => Text("Header"));
            foreach (var i in Enumerable.Range(0, 50_000))
            {
                P(child: () => Text($"Article_{i}"));
            }
        });

    [Composable]
    public static void FetchData_ScopedState()
    {
        var data = Remember.Get(Array.Empty<int>);
        Remember.LaunchedEffect(async () => { data.Value = await GetData() ?? Array.Empty<int>(); });

        P(child: () => Text("paragraph"));

        Div(child: () =>
        {
            if (data.Value.Length == 0)
                P(atr => atr.Id("no_data"), () => Text("not data"));
            else
                P(atr => atr.Id("data"), () => Text(string.Join(" ", data)));
        });
    }

    [Composable]
    public static void FetchData_NoScopedState()
    {
        var data = Remember.Get(Array.Empty<int>);
        Remember.LaunchedEffect(async () => data.Value = await GetData() ?? Array.Empty<int>());

        P(child: () => Text("paragraph"));

        if (data.Value.Length == 0)
            P(atr => atr.Id("no_data"), () => Text("not data"));
        else
            P(atr => atr.Id("data"), () => Text(string.Join(" ", data)));
    }

    public static void Counter() => Box(content: () =>
    {
        var counter = Remember.Get(0);

        H1(child: () => Text("Counter"));

        Div(child: () =>
        {
            Button(() => counter.Value++,
                attr => attr.Class("btn", "btn-primary"),
                () => Text($"Counter: {counter.Value}"));
        });
    });

    private static async Task<int[]?> GetData()
    {
        await Task.Delay(100);

        return new[] {1, 2, 3};
    }
}