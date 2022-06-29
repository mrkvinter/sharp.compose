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

    public MutableState<T> SetRemembered<T>(string key, T value, Action? onRemember = null, Action? onForgotten = null)
    {
        var v = new MutableState<T>(value, onForgotten);
        remembered[key] = v;

        return v;
    }

    public void Clear()
    {
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
    private readonly Action? onForget;

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

    public MutableState(TValue value, Action? onForget = null)
    {
        this.onForget = onForget;
        thisRemembered = this;
        thisRemembered.InternalValue = value!;
    }

    public void Forget()
    {
        onForget?.Invoke();
    }

    object IState.InternalValue { get; set; } = null!;
}