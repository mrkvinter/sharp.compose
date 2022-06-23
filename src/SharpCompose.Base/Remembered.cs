using System.Diagnostics.CodeAnalysis;

namespace SharpCompose.Base;

public class Remembered
{
    private readonly Dictionary<string, IValueRemembered> remembered = new();

    public IEnumerable<object?> RememberedValues => remembered.Values.Select(e => e.InternalValue);

    public bool TryGetNextRemembered<T>(string key, [MaybeNullWhen(false)] out ValueRemembered<T> result)
    {
        if (remembered.ContainsKey(key) && remembered[key] is ValueRemembered<T> val)
        {
            result = val;

            return true;
        }

        result = default;
        return false;
    }

    public ValueRemembered<T> AddRemembered<T>(string key, T value, Action? onRemember = null, Action? onForgotten = null)
    {
        var v = new ValueRemembered<T>(value, onForgotten);
        remembered.Add(key, v);

        return v;
    }

    public ValueRemembered<T> SetRemembered<T>(string key, T value, Action? onRemember = null, Action? onForgotten = null)
    {
        var v = new ValueRemembered<T>(value, onForgotten);
        remembered[key] = v;

        return v;
    }

    public void Clear()
    {
        remembered.Clear();
    }
}

internal interface IValueRemembered
{
    public object InternalValue { get; set; }
}

public class ValueRemembered<TValue> : IValueRemembered
{
    private readonly IValueRemembered thisRemembered;
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
    public ValueRemembered(TValue value, Action? onForget = null)
    {
        this.onForget = onForget;
        thisRemembered = this;
        thisRemembered.InternalValue = value!;
    }

    public void Forget()
    {
        onForget?.Invoke();
    }

    object IValueRemembered.InternalValue { get; set; } = null!;
}