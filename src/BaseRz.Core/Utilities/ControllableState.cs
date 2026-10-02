using Microsoft.AspNetCore.Components;

namespace BaseRz.Core.Utilities;

/// <summary>
/// Value that is either owned by the parent (controlled) or by the component (uncontrolled, seeded from a
/// <c>Default*</c> parameter). Call <see cref="Sync"/> from <c>OnParametersSet</c> and <see cref="SetAsync"/>
/// from user interaction.
/// </summary>
public sealed class ControllableState<T>
{
    private readonly IEqualityComparer<T> _comparer;
    private EventCallback<T> _changed;
    private bool _seeded;

    public ControllableState(IEqualityComparer<T>? comparer = null)
    {
        _comparer = comparer ?? EqualityComparer<T>.Default;
    }

    public T Value { get; private set; } = default!;

    public bool IsControlled { get; private set; }

    public void Sync(bool isControlled, T controlledValue, T defaultValue, EventCallback<T> changed)
    {
        IsControlled = isControlled;
        _changed = changed;

        if (isControlled)
        {
            Value = controlledValue;
            _seeded = true;
        }
        else if (!_seeded)
        {
            Value = defaultValue;
            _seeded = true;
        }
    }

    /// <summary>
    /// Requests a new value. Uncontrolled state updates immediately; controlled state only raises the
    /// changed callback and waits for the parent to pass the value back.
    /// </summary>
    /// <returns><see langword="true"/> when the value differed and the callback was raised.</returns>
    public async Task<bool> SetAsync(T value)
    {
        if (_comparer.Equals(Value, value))
        {
            return false;
        }

        if (!IsControlled)
        {
            Value = value;
        }

        await _changed.InvokeAsync(value);
        return true;
    }
}
