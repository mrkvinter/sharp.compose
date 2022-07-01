using System.Diagnostics.CodeAnalysis;

namespace SharpCompose.Base;

public class Remembered
{
    private readonly Dictionary<string, object> remembered = new();

    public IEnumerable<object?> RememberedValues => remembered.Values.Select(e => e);

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
    private readonly HashSet<Composer.Scope> scopeToChange = new();

    public TValue Value
    {
        get
        {
            if (Composer.Instance.Current != null) scopeToChange.Add(Composer.Instance.Current);
            return (TValue) thisRemembered.InternalValue;
        }
        set
        {
            if (thisRemembered.InternalValue.Equals(value))
                return;

            thisRemembered.InternalValue = value!;
            foreach (var scope in scopeToChange)
            {
                SetChange(scope);
            }

            Composer.Recompose();
        }
    }

    private void SetChange(Composer.Scope scope)
    {
        scope.Changed = true;
        foreach (var scopeChild in scope.Children)
        {
            SetChange(scopeChild);
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