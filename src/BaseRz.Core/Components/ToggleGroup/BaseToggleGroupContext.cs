using BaseRz.Core.Utilities;

namespace BaseRz.Core.Components.ToggleGroup;

/// <summary>Pressed values and roving focus shared by the toggle group items.</summary>
internal sealed class BaseToggleGroupContext(Action changed, Func<string, Task> toggle)
{
    public RovingFocusGroup Focus { get; } = new(changed);

    public SelectionMode Type { get; set; }

    public string? Value { get; set; }

    public IReadOnlyCollection<string> Values { get; set; } = [];

    public bool Disabled { get; set; }

    public bool RovingFocus { get; set; }

    public Orientation Orientation { get; set; }

    public bool IsPressed(string value) =>
        Type == SelectionMode.Multiple ? ValueSet.Contains(Values, value) : string.Equals(Value, value, StringComparison.Ordinal);

    public Task ToggleAsync(string value) => toggle(value);
}
