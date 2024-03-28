namespace SharpCompose.Base;

[AttributeUsage(AttributeTargets.Method)]
public class ComposableAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public class UiComposableAttribute : Attribute
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

[AttributeUsage(AttributeTargets.Method)]
public class GroupRootComposableApiAttribute : ComposableApiAttribute
{
}