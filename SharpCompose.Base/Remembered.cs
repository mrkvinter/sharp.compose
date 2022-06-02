using System.Diagnostics.CodeAnalysis;

namespace SharpCompose.Base;

public class Remembered
{
    private readonly List<IValueRemembered> remembered = new();
    private int rememberedIndex;

    public IEnumerable<object?> RememberedValues => remembered.Select(e => e.InternalValue);
    
    public bool HasNextRemembered<T>() =>
        rememberedIndex < remembered.Count && remembered[rememberedIndex].InternalValue is T;

    public bool TryGetNextRemembered<T>([MaybeNullWhen(false)] out ValueRemembered<T> result)
    {
        if (rememberedIndex < remembered.Count && remembered[rememberedIndex] is ValueRemembered<T> val)
        {
            result = val;
            rememberedIndex++;

            return true;
        }

        result = default;
        return false;
    }

    public ValueRemembered<T> AddRemembered<T>(T value)
    {
        var v = new ValueRemembered<T>(value);
        remembered.Add(v);
        rememberedIndex++;

        return v;
    }

    public ValueRemembered<T> NextRemembered<T>()
    {
        var value = remembered[rememberedIndex];
        rememberedIndex++;

        return (ValueRemembered<T>) value;
    }

    public void ResetRememberedIndex() => rememberedIndex = 0;

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

    public TValue Value
    {
        get
        {
            if (Composer.Instance.Current != null) scopeToChange.Add(Composer.Instance.Current);
            return (TValue) thisRemembered.InternalValue;
        }
        set
        {
            thisRemembered.InternalValue = value!;
            foreach (var scope in scopeToChange)
            {
                scope.Changed = true;
            }

            Composer.Recompose();
        }
    }

    public ValueRemembered(TValue value)
    {
        thisRemembered = this;
        thisRemembered.InternalValue = value!;
    }

    object IValueRemembered.InternalValue { get; set; } = null!;
}