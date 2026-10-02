using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.TreeView;

/// <summary>
/// APG single-select tree (<c>role="tree"</c>). Owns the selected value (<c>@bind-Value</c> or
/// <see cref="DefaultValue"/>) and the expanded values (<c>@bind-ExpandedValues</c> or
/// <see cref="DefaultExpandedValues"/>). Give it an accessible name with <c>aria-label</c> or <c>aria-labelledby</c>.
/// </summary>
public class BaseTreeViewRoot : BaseRzComponent
{
    private readonly ControllableState<string?> _value = new(StringComparer.Ordinal);
    private readonly ControllableState<IReadOnlyCollection<string>> _expanded = new(ValueSet.Comparer);
    private BaseTreeViewContext _context = default!;

    [Parameter] public string? Value { get; set; }

    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    [Parameter] public string? DefaultValue { get; set; }

    [Parameter] public IReadOnlyCollection<string>? ExpandedValues { get; set; }

    [Parameter] public EventCallback<IReadOnlyCollection<string>> ExpandedValuesChanged { get; set; }

    [Parameter] public IReadOnlyCollection<string>? DefaultExpandedValues { get; set; }

    protected override string DefaultElement => "ul";

    protected override void OnInitialized()
    {
        _context = new BaseTreeViewContext(() => InvokeAsync(StateHasChanged), SelectAsync, SetExpandedAsync);
    }

    protected override void OnParametersSet()
    {
        _value.Sync(IsParameterSet(nameof(Value)), Value, DefaultValue, ValueChanged);
        _expanded.Sync(IsParameterSet(nameof(ExpandedValues)), ExpandedValues ?? [], DefaultExpandedValues ?? [], ExpandedValuesChanged);
        UpdateContext();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddAttribute(1, "role", "tree");
        builder.AddMultipleAttributes(2, AdditionalAttributes);

        builder.OpenComponent<CascadingValue<BaseTreeViewContext>>(3);
        builder.AddComponentParameter(4, "Value", _context);
        builder.AddComponentParameter(5, "ChildContent", ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }

    private void UpdateContext()
    {
        _context.Value = _value.Value;
        _context.ExpandedValues = _expanded.Value ?? [];
    }

    private async Task SelectAsync(string value)
    {
        await _value.SetAsync(value);
        UpdateContext();
        StateHasChanged();
    }

    private async Task SetExpandedAsync(string value, bool expanded)
    {
        await _expanded.SetAsync(ValueSet.With(_expanded.Value, value, expanded));
        UpdateContext();
        StateHasChanged();
    }
}
