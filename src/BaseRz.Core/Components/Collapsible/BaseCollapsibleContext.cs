using BaseRz.Core.Utilities;

namespace BaseRz.Core.Components.Collapsible;

/// <summary>Open state and the trigger/content id pair shared by the collapsible parts.</summary>
internal sealed class BaseCollapsibleContext(string rootId, Action changed, Func<Task> toggle)
{
    public PartId TriggerId { get; } = new($"{rootId}-trigger", changed);

    public PartId ContentId { get; } = new($"{rootId}-content", changed);

    public bool Open { get; set; }

    public bool Disabled { get; set; }

    public Task ToggleAsync() => toggle();
}
