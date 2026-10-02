using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.RadioGroup;

/// <summary>
/// <c>role="radiogroup"</c> of custom (non-native) radios. Owns the checked value (<c>@bind-Value</c> or
/// <see cref="DefaultValue"/>). The group is one tab stop; arrow keys move focus and check. Set <see cref="Name"/>
/// to post the value with a hidden input.
/// </summary>
public class BaseRadioGroupRoot : BaseRzComponent
{
    private readonly ControllableState<string?> _value = new(StringComparer.Ordinal);
    private BaseRadioGroupContext _context = default!;

    [Parameter] public string? Value { get; set; }

    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    [Parameter] public string? DefaultValue { get; set; }

    [Parameter] public bool Disabled { get; set; }

    [Parameter] public bool Required { get; set; }

    /// <summary>Form field name; when set a hidden input carries the checked value.</summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>Arrow-key axis; <see langword="null"/> (the default) accepts both axes.</summary>
    [Parameter] public Orientation? Orientation { get; set; }

    [Parameter] public Direction Direction { get; set; } = Direction.Ltr;

    [Parameter] public bool Loop { get; set; } = true;

    protected override void OnInitialized()
    {
        _context = new BaseRadioGroupContext(() => InvokeAsync(StateHasChanged), SelectAsync);
    }

    protected override void OnParametersSet()
    {
        _value.Sync(IsParameterSet(nameof(Value)), Value, DefaultValue, ValueChanged);
        _context.Value = _value.Value;
        _context.Disabled = Disabled;
        _context.Focus.Orientation = Orientation;
        _context.Focus.Direction = Direction;
        _context.Focus.Loop = Loop;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddAttribute(1, "role", "radiogroup");
        builder.AddMultipleAttributes(2, AdditionalAttributes);
        if (Orientation is { } orientation)
        {
            builder.AddAttribute(3, "aria-orientation", DataAttributes.Of(orientation));
            builder.AddDataOrientation(4, orientation);
        }

        if (Required)
        {
            builder.AddAttribute(5, "aria-required", "true");
        }

        if (Disabled)
        {
            builder.AddAttribute(6, "aria-disabled", "true");
        }

        builder.AddDataDisabled(7, Disabled);

        builder.OpenComponent<CascadingValue<BaseRadioGroupContext>>(8);
        builder.AddComponentParameter(9, "Value", _context);
        builder.AddComponentParameter(10, "ChildContent", ChildContent);
        builder.CloseComponent();
        builder.AddHiddenInput(11, Name, _value.Value);
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
