using BaseRz.Core.Utilities;

namespace BaseRz.Core.Components.RadioGroup;

/// <summary>Checked value and roving focus shared by the radio items.</summary>
internal sealed class BaseRadioGroupContext
{
    private readonly Func<string, Task> _select;

    public BaseRadioGroupContext(Action changed, Func<string, Task> select)
    {
        _select = select;
        Focus = new RovingFocusGroup(changed)
        {
            Preferred = () => Focus!.Items.FirstOrDefault(item => string.Equals(item.Value, Value, StringComparison.Ordinal)),
        };
    }

    public RovingFocusGroup Focus { get; }

    public string? Value { get; set; }

    public bool Disabled { get; set; }

    public bool IsChecked(string value) => string.Equals(Value, value, StringComparison.Ordinal);

    public Task SelectAsync(string value) => _select(value);
}

/// <summary>Per-item state read by <see cref="BaseRadioGroupIndicator"/>.</summary>
internal sealed record BaseRadioGroupItemState(bool Checked, bool Disabled);
