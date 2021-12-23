namespace SharpCompose.Base;

public class Remembered
{
    private readonly List<IValueRemembered> remembered = new();
    private int rememberedIndex;

    public bool HasNextRemembered<T>() => rememberedIndex < remembered.Count && remembered[rememberedIndex].InternalValue is T;

    public ValueRemembered<T> AddRemembered<T>(T value)
    {
        var v = new ValueRemembered<T>(value);
        remembered.Add(v);

        return v;
    }

    public ValueRemembered<T> NextRemembered<T>()
    {
        var value = remembered[rememberedIndex];
        rememberedIndex++;

        return (ValueRemembered<T>)value;
    }

    public void ResetRememberedIndex() => rememberedIndex = 0;


    private interface IValueRemembered
    {
        public object InternalValue { get; set; }
    }

    public class ValueRemembered<TValue> : IValueRemembered
    {
        private readonly IValueRemembered thisRemembered;

        public TValue Value
        {
            get => (TValue)thisRemembered.InternalValue;
            set
            {
                thisRemembered.InternalValue = value!;
                Composer.Recompose();
            }
        }

        public ValueRemembered(TValue value)
        {
            thisRemembered = this;
            thisRemembered.InternalValue = value ?? throw new ArgumentNullException(nameof(value));
        }

        object IValueRemembered.InternalValue
        {
            get;
            set;
        } = null!;
    }
}