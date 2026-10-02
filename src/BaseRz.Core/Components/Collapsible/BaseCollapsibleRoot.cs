using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Collapsible;

/// <summary>
/// Disclosure container. Owns the open state (<c>@bind-Open</c> or <see cref="DefaultOpen"/>) and wires
/// <see cref="BaseCollapsibleTrigger"/> to <see cref="BaseCollapsibleContent"/>.
/// </summary>
public class BaseCollapsibleRoot : BaseRzComponent
{
    private readonly ControllableState<bool> _open = new();
    private BaseCollapsibleContext _context = default!;

    [Parameter] public bool Open { get; set; }

    [Parameter] public EventCallback<bool> OpenChanged { get; set; }

    [Parameter] public bool DefaultOpen { get; set; }

    [Parameter] public bool Disabled { get; set; }

    protected override string IdPrefix => "baserz-collapsible";

    protected override void OnInitialized()
    {
        _context = new BaseCollapsibleContext(Id, () => InvokeAsync(StateHasChanged), ToggleAsync);
    }

    protected override void OnParametersSet()
    {
        _open.Sync(IsParameterSet(nameof(Open)), Open, DefaultOpen, OpenChanged);
        _context.Open = _open.Value;
        _context.Disabled = Disabled;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddDataState(2, _open.Value);
        builder.AddDataDisabled(3, Disabled);

        builder.OpenComponent<CascadingValue<BaseCollapsibleContext>>(4);
        builder.AddComponentParameter(5, "Value", _context);
        builder.AddComponentParameter(6, "ChildContent", ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }

    private async Task ToggleAsync()
    {
        if (Disabled)
        {
            return;
        }

        await _open.SetAsync(!_open.Value);
        _context.Open = _open.Value;
        StateHasChanged();
    }
}
