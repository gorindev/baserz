using BaseRz.Core.Utilities;

namespace BaseRz.Core.Components.Tabs;

/// <summary>Selected value, tab focus, and the per-value tab/panel id pairs shared by the tabs parts.</summary>
internal sealed class BaseTabsContext
{
    private readonly string _rootId;
    private readonly Action _changed;
    private readonly Func<string, Task> _select;
    private readonly Dictionary<string, string> _triggerIds = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _contentIds = new(StringComparer.Ordinal);

    public BaseTabsContext(string rootId, Action changed, Func<string, Task> select)
    {
        _rootId = rootId;
        _changed = changed;
        _select = select;
        Focus = new RovingFocusGroup(changed)
        {
            Preferred = () => Focus!.Items.FirstOrDefault(item => string.Equals(item.Value, Value, StringComparison.Ordinal)),
        };
    }

    public RovingFocusGroup Focus { get; }

    public string? Value { get; set; }

    public Orientation Orientation { get; set; }

    public bool Disabled { get; set; }

    public bool IsSelected(string value) => string.Equals(Value, value, StringComparison.Ordinal);

    public string TriggerId(string value) => _triggerIds.TryGetValue(value, out var id) ? id : $"{_rootId}-trigger-{Sanitize(value)}";

    public string ContentId(string value) => _contentIds.TryGetValue(value, out var id) ? id : $"{_rootId}-content-{Sanitize(value)}";

    public void UseTriggerId(string value, Dictionary<string, object>? attributes) => Use(_triggerIds, value, attributes);

    public void UseContentId(string value, Dictionary<string, object>? attributes) => Use(_contentIds, value, attributes);

    public Task SelectAsync(string value) => _select(value);

    private void Use(Dictionary<string, string> ids, string value, Dictionary<string, object>? attributes)
    {
        var callerId = PartId.CallerId(attributes);
        ids.TryGetValue(value, out var current);
        if (string.Equals(current, callerId, StringComparison.Ordinal))
        {
            return;
        }

        if (callerId is null)
        {
            ids.Remove(value);
        }
        else
        {
            ids[value] = callerId;
        }

        _changed();
    }

    private static string Sanitize(string value) => new(value.Select(static c => char.IsLetterOrDigit(c) ? c : '-').ToArray());
}
