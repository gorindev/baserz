using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Tabs;

/// <summary>
/// Tabs container. Owns the selected value (<c>@bind-Value</c> or <see cref="DefaultValue"/>). Activation is
/// automatic: moving focus with the arrow keys, Home or End selects the focused tab.
/// </summary>
public class BaseTabsRoot : BaseRzComponent
{
    private readonly ControllableState<string?> _value = new(StringComparer.Ordinal);
    private BaseTabsContext _context = default!;

    [Parameter] public string? Value { get; set; }

    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    [Parameter] public string? DefaultValue { get; set; }

    [Parameter] public Orientation Orientation { get; set; } = Orientation.Horizontal;

    [Parameter] public Direction Direction { get; set; } = Direction.Ltr;

    [Parameter] public bool Loop { get; set; } = true;

    [Parameter] public bool Disabled { get; set; }

    protected override string IdPrefix => "baserz-tabs";

    protected override void OnInitialized()
    {
        _context = new BaseTabsContext(Id, () => InvokeAsync(StateHasChanged), SelectAsync);
    }

    protected override void OnParametersSet()
    {
        _value.Sync(IsParameterSet(nameof(Value)), Value, DefaultValue, ValueChanged);
        _context.Value = _value.Value;
        _context.Orientation = Orientation;
        _context.Disabled = Disabled;
        _context.Focus.Orientation = Orientation;
        _context.Focus.Direction = Direction;
        _context.Focus.Loop = Loop;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddDataOrientation(2, Orientation);
        builder.AddDataDisabled(3, Disabled);

        builder.OpenComponent<CascadingValue<BaseTabsContext>>(4);
        builder.AddComponentParameter(5, "Value", _context);
        builder.AddComponentParameter(6, "ChildContent", ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }

    private async Task SelectAsync(string value)
    {
        if (Disabled)
        {
            return;
        }

        await _value.SetAsync(value);
        _context.Value = _value.Value;
        StateHasChanged();
    }
}
