# Sharp.Compose

SharpCompose is a modern way of creating UI by C#. This framework was inspired of Jetpack Compose.

```csharp
[Composable]
private static void Counter() => Column(content: () =>
{
    var counter = Remember.Get(() => 0.AsMutableState());

    Text($"Current count: {counter.Value}");
    Button(() => counter.Value++, "Click me");
});
```

## Example

You can explore example project [here](https://github.com/mrkvinter/SharpCompose.ExampleApp).

## Disclaimer

It is early version of framework, so many things can will change in the future. Use this at your own risk.