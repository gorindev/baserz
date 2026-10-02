using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Pagination;

/// <summary>
/// Landmark <c>&lt;nav&gt;</c> labeled "Pagination". Owns the current page (<c>@bind-Page</c> or
/// <see cref="DefaultPage"/>) and hands a <see cref="BasePaginationContext"/> to its child content so callers can
/// render <see cref="BasePaginationLink"/> / <see cref="BasePaginationEllipsis"/> from <see cref="BasePaginationContext.Items"/>.
/// </summary>
public class BasePaginationRoot : BaseRzTemplatedComponent<BasePaginationContext>
{
    public const string DefaultLabel = "Pagination";

    private readonly ControllableState<int> _page = new();
    private readonly BasePaginationContext _context;

    public BasePaginationRoot()
    {
        _context = new BasePaginationContext(SetPageAsync);
    }

    [Parameter] public int Page { get; set; } = 1;

    [Parameter] public EventCallback<int> PageChanged { get; set; }

    [Parameter] public int DefaultPage { get; set; } = 1;

    [Parameter] public int PageCount { get; set; }

    /// <summary>Pages shown on each side of the current page before collapsing into an ellipsis.</summary>
    [Parameter] public int SiblingCount { get; set; } = 1;

    [Parameter] public bool Disabled { get; set; }

    protected override string DefaultElement => "nav";

    protected override void OnParametersSet()
    {
        _page.Sync(IsParameterSet(nameof(Page)), Page, DefaultPage, PageChanged);
        UpdateContext();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        if (!HasAttribute("aria-label") && !HasAttribute("aria-labelledby"))
        {
            builder.AddAttribute(1, "aria-label", DefaultLabel);
        }

        builder.AddMultipleAttributes(2, AdditionalAttributes);
        builder.AddDataDisabled(3, Disabled);

        builder.OpenComponent<CascadingValue<BasePaginationContext>>(4);
        builder.AddComponentParameter(5, "Value", _context);
        builder.AddComponentParameter(6, "ChildContent", ChildContent?.Invoke(_context));
        builder.CloseComponent();
        builder.CloseElement();
    }

    /// <summary>
    /// First page, <paramref name="siblingCount"/> pages either side of <paramref name="page"/>, and the last page,
    /// with <see langword="null"/> for each gap. A gap of exactly one page shows that page instead of an ellipsis.
    /// </summary>
    public static IReadOnlyList<int?> Range(int page, int pageCount, int siblingCount)
    {
        if (pageCount <= 0)
        {
            return [];
        }

        var current = Math.Clamp(page, 1, pageCount);
        var siblings = Math.Max(0, siblingCount);
        var pages = new SortedSet<int> { 1, pageCount };
        for (var i = Math.Max(2, current - siblings); i <= Math.Min(pageCount - 1, current + siblings); i++)
        {
            pages.Add(i);
        }

        var items = new List<int?>(pages.Count + 2);
        var previous = 0;
        foreach (var value in pages)
        {
            var gap = value - previous;
            if (previous > 0 && gap == 2)
            {
                items.Add(previous + 1);
            }
            else if (previous > 0 && gap > 2)
            {
                items.Add(null);
            }

            items.Add(value);
            previous = value;
        }

        return items;
    }

    private void UpdateContext()
    {
        _context.PageCount = Math.Max(0, PageCount);
        _context.Page = Math.Clamp(_page.Value, 1, Math.Max(1, _context.PageCount));
        _context.Disabled = Disabled;
        _context.Items = Range(_context.Page, _context.PageCount, SiblingCount);
    }

    private async Task SetPageAsync(int page)
    {
        if (Disabled || _context.PageCount == 0)
        {
            return;
        }

        var next = Math.Clamp(page, 1, _context.PageCount);
        if (next == _context.Page)
        {
            return;
        }

        await _page.SetAsync(next);
        UpdateContext();
        StateHasChanged();
    }
}
