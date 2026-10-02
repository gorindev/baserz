using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Utilities;

/// <summary>A focusable member of a <see cref="RovingFocusGroup"/>, registered in document order.</summary>
internal sealed class RovingFocusItem(Func<bool> isDisabled, Func<Task> focusAsync, string? value = null)
{
    public Func<bool> IsDisabled { get; } = isDisabled;

    public Func<Task> FocusAsync { get; } = focusAsync;

    public string? Value { get; set; } = value;
}

/// <summary>
/// Roving tabindex: one item is the tab stop (<c>tabindex="0"</c>), the rest are <c>-1</c>. Arrow keys (by
/// <see cref="Orientation"/>, mirrored for RTL), Home and End move focus between enabled items.
/// </summary>
internal sealed class RovingFocusGroup(Action changed)
{
    private readonly List<RovingFocusItem> _items = [];
    private RovingFocusItem? _active;

    /// <summary>Axis that arrow keys follow; <see langword="null"/> accepts both axes.</summary>
    public Orientation? Orientation { get; set; }

    public Direction Direction { get; set; } = Direction.Ltr;

    public bool Loop { get; set; } = true;

    /// <summary>Item that should be the tab stop when it is enabled (for example the selected tab).</summary>
    public Func<RovingFocusItem?>? Preferred { get; set; }

    /// <summary>Hook for character search; receives the key and returns the item to focus.</summary>
    public Func<string, RovingFocusItem?>? Typeahead { get; set; }

    public IReadOnlyList<RovingFocusItem> Items => _items;

    public static bool IsNavigationKey(KeyboardEventArgs args) =>
        args.Key is "ArrowUp" or "ArrowDown" or "ArrowLeft" or "ArrowRight" or "Home" or "End";

    public void Register(RovingFocusItem item)
    {
        if (!_items.Contains(item))
        {
            _items.Add(item);
        }
    }

    public void Unregister(RovingFocusItem item)
    {
        _items.Remove(item);
        if (ReferenceEquals(_active, item))
        {
            _active = null;
        }
    }

    public RovingFocusItem? Active
    {
        get
        {
            var preferred = Preferred?.Invoke();
            if (preferred is not null && IsFocusable(preferred))
            {
                return preferred;
            }

            return _active is not null && IsFocusable(_active) ? _active : _items.Find(static item => !item.IsDisabled());
        }
    }

    public bool IsActive(RovingFocusItem item) => ReferenceEquals(Active, item);

    public string TabIndexFor(RovingFocusItem item) => IsActive(item) ? "0" : "-1";

    public void SetActive(RovingFocusItem item)
    {
        if (ReferenceEquals(_active, item))
        {
            return;
        }

        _active = item;
        changed();
    }

    /// <summary>Moves focus for a navigation (or typeahead) key. Returns the newly focused item, or <see langword="null"/> when the key does not move focus.</summary>
    public async Task<RovingFocusItem?> HandleKeyDownAsync(RovingFocusItem current, KeyboardEventArgs args)
    {
        var target = Resolve(current, args);
        if (target is null)
        {
            return null;
        }

        SetActive(target);
        await target.FocusAsync();
        return target;
    }

    private RovingFocusItem? Resolve(RovingFocusItem current, KeyboardEventArgs args)
    {
        var horizontal = Orientation is null or Core.Orientation.Horizontal;
        var vertical = Orientation is null or Core.Orientation.Vertical;
        var rtl = Direction == Direction.Rtl;

        return args.Key switch
        {
            "Home" => _items.Find(static item => !item.IsDisabled()),
            "End" => _items.FindLast(static item => !item.IsDisabled()),
            "ArrowDown" when vertical => Step(current, 1),
            "ArrowUp" when vertical => Step(current, -1),
            "ArrowRight" when horizontal => Step(current, rtl ? -1 : 1),
            "ArrowLeft" when horizontal => Step(current, rtl ? 1 : -1),
            { Length: 1 } key when Typeahead is not null => Typeahead(key),
            _ => null,
        };
    }

    private RovingFocusItem? Step(RovingFocusItem current, int offset)
    {
        var index = _items.IndexOf(current);
        if (index < 0)
        {
            return null;
        }

        for (var i = 1; i <= _items.Count; i++)
        {
            var next = index + (offset * i);
            if (Loop)
            {
                next = ((next % _items.Count) + _items.Count) % _items.Count;
            }
            else if (next < 0 || next >= _items.Count)
            {
                return null;
            }

            var candidate = _items[next];
            if (ReferenceEquals(candidate, current))
            {
                return null;
            }

            if (!candidate.IsDisabled())
            {
                return candidate;
            }
        }

        return null;
    }

    private bool IsFocusable(RovingFocusItem item) => _items.Contains(item) && !item.IsDisabled();
}
