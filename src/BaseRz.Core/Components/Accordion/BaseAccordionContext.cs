using BaseRz.Core.Utilities;

namespace BaseRz.Core.Components.Accordion;

/// <summary>Open values, options, and trigger focus navigation shared by every accordion item.</summary>
internal sealed class BaseAccordionContext(Action changed, Func<string, Task> toggle)
{
    public RovingFocusGroup Focus { get; } = new(changed);

    public SelectionMode Type { get; set; }

    public bool Collapsible { get; set; }

    public bool Disabled { get; set; }

    public Orientation Orientation { get; set; }

    public string? Value { get; set; }

    public IReadOnlyCollection<string> Values { get; set; } = [];

    public Action Changed => changed;

    public bool IsOpen(string value) =>
        Type == SelectionMode.Multiple ? ValueSet.Contains(Values, value) : string.Equals(Value, value, StringComparison.Ordinal);

    /// <summary>An open item that cannot be closed by its own trigger (single mode, not collapsible).</summary>
    public bool IsLocked(string value) => Type == SelectionMode.Single && !Collapsible && IsOpen(value);

    public Task ToggleAsync(string value) => toggle(value);
}

/// <summary>Per-item state: open/disabled plus the trigger/content id pair.</summary>
internal sealed class BaseAccordionItemContext(BaseAccordionContext root, BaseAccordionItem item, string itemId)
{
    public BaseAccordionContext Root => root;

    public PartId TriggerId { get; } = new($"{itemId}-trigger", root.Changed);

    public PartId ContentId { get; } = new($"{itemId}-content", root.Changed);

    public string Value => item.Value;

    public bool Open => root.IsOpen(item.Value);

    public bool Disabled => root.Disabled || item.Disabled;

    public bool Locked => root.IsLocked(item.Value);
}
