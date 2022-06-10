namespace SharpCompose.Base;

[AttributeUsage(AttributeTargets.Method)]
public class ComposableAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public class RootComposableAttribute : Attribute
{
}