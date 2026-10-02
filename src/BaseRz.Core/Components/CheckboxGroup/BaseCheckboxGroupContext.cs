using BaseRz.Core.Utilities;

namespace BaseRz.Core.Components.CheckboxGroup;

/// <summary>Checked values plus the label/description/error ids the group root references.</summary>
internal sealed class BaseCheckboxGroupContext(Action changed, Func<string, bool, Task> set)
{
    public RegisteredId LabelId { get; } = new(changed);

    public RegisteredId DescriptionId { get; } = new(changed);

    public RegisteredId ErrorMessageId { get; } = new(changed);

    public IReadOnlyCollection<string> Values { get; set; } = [];

    public bool Disabled { get; set; }

    public string? Name { get; set; }

    public bool Contains(string value) => ValueSet.Contains(Values, value);

    public Task SetAsync(string value, bool isChecked) => set(value, isChecked);
}
