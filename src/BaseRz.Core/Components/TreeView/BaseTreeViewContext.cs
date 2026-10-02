using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.TreeView;

/// <summary>A mounted tree item: its place in the hierarchy plus focus and label wiring.</summary>
internal sealed class BaseTreeViewNode(BaseTreeViewContext root, BaseTreeViewNode? parent, BaseTreeViewItem item, string itemId, Func<Task> focusAsync)
{
    public BaseTreeViewContext Root => root;

    public BaseTreeViewNode? Parent => parent;

    public List<BaseTreeViewNode> Children { get; } = [];

    public int Level => (parent?.Level ?? 0) + 1;

    public string Value => item.Value;

    public bool Disabled => item.Disabled;

    public PartId LabelId { get; } = new($"{itemId}-label", root.Changed);

    public bool HasLabel { get; set; }

    /// <summary>Set while a <see cref="BaseTreeViewContent"/> is mounted, which makes the item expandable.</summary>
    public bool HasContent { get; set; }

    public bool Expanded => HasContent && root.IsExpanded(item.Value);

    public bool Selected => root.IsSelected(item.Value);

    public Task FocusAsync() => focusAsync();
}

/// <summary>Selection, expansion, and keyboard navigation over the visible tree items.</summary>
internal sealed class BaseTreeViewContext(
    Action changed,
    Func<string, Task> select,
    Func<string, bool, Task> setExpanded)
{
    private BaseTreeViewNode? _active;

    public List<BaseTreeViewNode> Roots { get; } = [];

    public string? Value { get; set; }

    public IReadOnlyCollection<string> ExpandedValues { get; set; } = [];

    public Action Changed => changed;

    public bool IsSelected(string value) => string.Equals(Value, value, StringComparison.Ordinal);

    public bool IsExpanded(string value) => ValueSet.Contains(ExpandedValues, value);

    public void Add(BaseTreeViewNode node) => (node.Parent?.Children ?? Roots).Add(node);

    public void Remove(BaseTreeViewNode node)
    {
        (node.Parent?.Children ?? Roots).Remove(node);
        if (ReferenceEquals(_active, node))
        {
            _active = null;
        }
    }

    /// <summary>Visible items in document order: roots, then the children of expanded items.</summary>
    public List<BaseTreeViewNode> Visible()
    {
        var visible = new List<BaseTreeViewNode>();
        Walk(Roots);
        return visible;

        void Walk(List<BaseTreeViewNode> nodes)
        {
            foreach (var node in nodes)
            {
                visible.Add(node);
                if (node.Expanded)
                {
                    Walk(node.Children);
                }
            }
        }
    }

    public string TabIndexFor(BaseTreeViewNode node) => ReferenceEquals(Active(), node) ? "0" : "-1";

    public void SetActive(BaseTreeViewNode node)
    {
        if (!ReferenceEquals(_active, node))
        {
            _active = node;
            changed();
        }
    }

    public Task SelectAsync(BaseTreeViewNode node)
    {
        if (node.Disabled)
        {
            return Task.CompletedTask;
        }

        SetActive(node);
        return select(node.Value);
    }

    public Task ToggleExpandedAsync(BaseTreeViewNode node) =>
        node.HasContent && !node.Disabled ? setExpanded(node.Value, !node.Expanded) : Task.CompletedTask;

    public async Task HandleKeyDownAsync(BaseTreeViewNode node, KeyboardEventArgs args)
    {
        var visible = Visible().Where(static candidate => !candidate.Disabled).ToList();
        var index = visible.IndexOf(node);

        switch (args.Key)
        {
            case "ArrowDown" when index >= 0 && index < visible.Count - 1:
                await FocusAsync(visible[index + 1]);
                break;
            case "ArrowUp" when index > 0:
                await FocusAsync(visible[index - 1]);
                break;
            case "Home" when visible.Count > 0:
                await FocusAsync(visible[0]);
                break;
            case "End" when visible.Count > 0:
                await FocusAsync(visible[^1]);
                break;
            case "ArrowRight" when node.HasContent && !node.Expanded:
                await setExpanded(node.Value, true);
                break;
            case "ArrowRight" when node.Expanded && node.Children.Find(static child => !child.Disabled) is { } child:
                await FocusAsync(child);
                break;
            case "ArrowLeft" when node.Expanded:
                await setExpanded(node.Value, false);
                break;
            case "ArrowLeft" when node.Parent is not null:
                await FocusAsync(node.Parent);
                break;
            case "Enter" or " ":
                await SelectAsync(node);
                break;
        }
    }

    private BaseTreeViewNode? Active()
    {
        var visible = Visible();
        if (_active is not null && !_active.Disabled && visible.Contains(_active))
        {
            return _active;
        }

        return visible.Find(static node => node.Selected && !node.Disabled) ?? visible.Find(static node => !node.Disabled);
    }

    private async Task FocusAsync(BaseTreeViewNode node)
    {
        SetActive(node);
        await node.FocusAsync();
    }
}
