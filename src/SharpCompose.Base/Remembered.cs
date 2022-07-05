using System.Diagnostics.CodeAnalysis;
using SharpCompose.Drawer.Core;

namespace SharpCompose.Base;

public class Remembered
{
    private readonly Dictionary<string, object> remembered = new();

    public bool TryGetNextRemembered<T>(string key, [MaybeNullWhen(false)] out T result)
    {
        if (remembered.ContainsKey(key) && remembered[key] is T val)
        {
            result = val;

            return true;
        }

        result = default;
        return false;
    }

    public void AddRemembered<T>(string key, T value) =>
        remembered.Add(key, value);

    public void RemoveRemembered(string key) =>
        remembered.Remove(key);

    public void Clear()
    {
        foreach (var (_, value) in remembered)
            if (value is IRememberObserver rememberObserver)
                rememberObserver.OnForgotten();

        remembered.Clear();
    }
}

internal interface IState
{
    public object InternalValue { get; set; }
}

public class MutableState<TValue> : IState
{
    private readonly IState thisRemembered;
    private readonly HashSet<INode> nodesToChange = new();

    public TValue Value
    {
        get
        {
            if (Composer.Instance.Current != null) nodesToChange.Add(Composer.Instance.Current);
            return (TValue) thisRemembered.InternalValue;
        }
        set
        {
            if (thisRemembered.InternalValue.Equals(value))
                return;

            thisRemembered.InternalValue = value!;
            foreach (var node in nodesToChange)
            {
                SetChange(node);
            }

            Composer.Recompose();
        }
    }

    private void SetChange(INode node)
    {
        switch (node)
        {
            case GroupNode groupNode:
            {
                foreach (var childGroupNode in groupNode.Nodes)
                {
                    childGroupNode.Changed = true;
                    childGroupNode.Children.ForEach(SetChange);
                }
                break;
            }
            case LayoutNode scope:
            {
                scope.Changed = true;
                scope.Children.ForEach(SetChange);
                break;
            }
        }
    }

    public MutableState(TValue value)
    {
        thisRemembered = this;
        thisRemembered.InternalValue = value!;
    }

    object IState.InternalValue { get; set; } = null!;

    public override string ToString() => $"MutableState<{typeof(TValue)}>({thisRemembered.InternalValue})";
}