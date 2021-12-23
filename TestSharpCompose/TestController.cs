using System.Linq;
using SharpCompose.Base;
using static SharpCompose.Base.ComposesApi.BaseCompose;
using static SharpCompose.Base.ComposesApi.HtmlCompose;

namespace TestSharpCompose;

public class TestController
{
    [Compose]
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


    [Compose]
    public static void ComplexCompose() =>
        Div(attr =>
        {
            attr.Id("root");
            attr.Class("text-center");
        }, () =>
        {
            H1(child: () => Text("Header"));
            foreach (var i in Enumerable.Range(0, 100_000))
            {
                P(child: () => Text($"Article_{i}"));
            }
        });
}