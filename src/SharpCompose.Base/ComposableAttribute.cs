namespace SharpCompose.Base;

[AttributeUsage(AttributeTargets.Method)]
public class ComposableAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public class ComposableApiAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public class RootComposableApiAttribute : ComposableApiAttribute
{
}