namespace BaseRz.Core.Components.Pagination;

/// <summary>Current pagination state, passed to the root's child content and cascaded to every part.</summary>
public sealed class BasePaginationContext
{
    private readonly Func<int, Task> _setPage;

    internal BasePaginationContext(Func<int, Task> setPage)
    {
        _setPage = setPage;
    }

    public int Page { get; internal set; } = 1;

    public int PageCount { get; internal set; }

    public bool Disabled { get; internal set; }

    /// <summary>Pages to render in order; <see langword="null"/> marks an ellipsis gap.</summary>
    public IReadOnlyList<int?> Items { get; internal set; } = [];

    public bool CanGoPrevious => !Disabled && Page > 1;

    public bool CanGoNext => !Disabled && Page < PageCount;

    /// <summary>Requests <paramref name="page"/> (clamped to the page range). Ignored when disabled.</summary>
    public Task SetPageAsync(int page) => _setPage(page);
}
