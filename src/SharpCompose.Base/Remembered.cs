using System.Diagnostics.CodeAnalysis;
using SharpCompose.Base.Nodes;

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
    private readonly HashSet<IGroupNode> nodesToChange = new();
    private TValue value;

    public TValue Value
    {
        get
        {
            if (Composer.Instance.PossibleCurrentGroup != null)
                nodesToChange.Add(Composer.Instance.PossibleCurrentGroup);

            return value;
        }
        set
        {
            if (this.value.Equals(value))
                return;

            this.value = value;

            foreach (var node in nodesToChange)
            {
                SetChange(node);
            }

            Composer.Recompose();
        }
    }

    private static void SetChange(IGroupNode groupNode)
    {
        foreach (var childGroupNode in groupNode.Nodes)
        {
            childGroupNode.Changed = true;
            SetChange(childGroupNode.GroupNode);
        }
    }

    public MutableState(TValue value)
    {
        this.value = value;
    }

    object IState.InternalValue { get; set; } = null!;

    public override string ToString() => $"MutableState<{typeof(TValue)}>({value})";
}