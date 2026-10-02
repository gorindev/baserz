using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Accordion;

/// <summary>
/// Stack of disclosure items. <see cref="Type"/> <c>Single</c> keeps one item open (<c>@bind-Value</c> or
/// <see cref="DefaultValue"/>); <c>Multiple</c> allows many (<c>@bind-Values</c> or <see cref="DefaultValues"/>).
/// Arrow keys, Home and End move focus between triggers without toggling.
/// </summary>
public class BaseAccordionRoot : BaseRzComponent
{
    private readonly ControllableState<string?> _value = new(StringComparer.Ordinal);
    private readonly ControllableState<IReadOnlyCollection<string>> _values = new(ValueSet.Comparer);
    private BaseAccordionContext _context = default!;

    [Parameter] public SelectionMode Type { get; set; } = SelectionMode.Single;

    [Parameter] public string? Value { get; set; }

    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    [Parameter] public string? DefaultValue { get; set; }

    [Parameter] public IReadOnlyCollection<string>? Values { get; set; }

    [Parameter] public EventCallback<IReadOnlyCollection<string>> ValuesChanged { get; set; }

    [Parameter] public IReadOnlyCollection<string>? DefaultValues { get; set; }

    /// <summary>In single mode, whether the open item can be closed again (leaving none open).</summary>
    [Parameter] public bool Collapsible { get; set; } = true;

    [Parameter] public bool Disabled { get; set; }

    [Parameter] public Orientation Orientation { get; set; } = Orientation.Vertical;

    [Parameter] public Direction Direction { get; set; } = Direction.Ltr;

    [Parameter] public bool Loop { get; set; } = true;

    protected override void OnInitialized()
    {
        _context = new BaseAccordionContext(() => InvokeAsync(StateHasChanged), ToggleAsync);
    }

    protected override void OnParametersSet()
    {
        _value.Sync(IsParameterSet(nameof(Value)), Value, DefaultValue, ValueChanged);
        _values.Sync(IsParameterSet(nameof(Values)), Values ?? [], DefaultValues ?? [], ValuesChanged);
        _context.Type = Type;
        _context.Collapsible = Collapsible;
        _context.Disabled = Disabled;
        _context.Orientation = Orientation;
        _context.Focus.Orientation = Orientation;
        _context.Focus.Direction = Direction;
        _context.Focus.Loop = Loop;
        UpdateContext();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddDataOrientation(2, Orientation);
        builder.AddDataDisabled(3, Disabled);

        builder.OpenComponent<CascadingValue<BaseAccordionContext>>(4);
        builder.AddComponentParameter(5, "Value", _context);
        builder.AddComponentParameter(6, "ChildContent", ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }

    private void UpdateContext()
    {
        _context.Value = _value.Value;
        _context.Values = _values.Value ?? [];
    }

    private async Task ToggleAsync(string value)
    {
        if (Disabled)
        {
            return;
        }

        if (Type == SelectionMode.Multiple)
        {
            await _values.SetAsync(ValueSet.Toggle(_values.Value, value));
        }
        else
        {
            var open = string.Equals(_value.Value, value, StringComparison.Ordinal);
            if (open && !Collapsible)
            {
                return;
            }

            await _value.SetAsync(open ? null : value);
        }

        UpdateContext();
        StateHasChanged();
    }
}
