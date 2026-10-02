using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.ToggleGroup;

/// <summary>
/// <c>role="group"</c> of toggle buttons. <see cref="Type"/> <c>Single</c> keeps at most one pressed
/// (<c>@bind-Value</c>); <c>Multiple</c> presses independently (<c>@bind-Values</c>). With <see cref="RovingFocus"/>
/// the group is one tab stop and arrow keys move between items.
/// </summary>
public class BaseToggleGroupRoot : BaseRzComponent
{
    private readonly ControllableState<string?> _value = new(StringComparer.Ordinal);
    private readonly ControllableState<IReadOnlyCollection<string>> _values = new(ValueSet.Comparer);
    private BaseToggleGroupContext _context = default!;

    [Parameter] public SelectionMode Type { get; set; } = SelectionMode.Single;

    [Parameter] public string? Value { get; set; }

    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    [Parameter] public string? DefaultValue { get; set; }

    [Parameter] public IReadOnlyCollection<string>? Values { get; set; }

    [Parameter] public EventCallback<IReadOnlyCollection<string>> ValuesChanged { get; set; }

    [Parameter] public IReadOnlyCollection<string>? DefaultValues { get; set; }

    [Parameter] public bool Disabled { get; set; }

    [Parameter] public Orientation Orientation { get; set; } = Orientation.Horizontal;

    [Parameter] public Direction Direction { get; set; } = Direction.Ltr;

    [Parameter] public bool Loop { get; set; } = true;

    [Parameter] public bool RovingFocus { get; set; } = true;

    protected override void OnInitialized()
    {
        _context = new BaseToggleGroupContext(() => InvokeAsync(StateHasChanged), ToggleAsync);
    }

    protected override void OnParametersSet()
    {
        _value.Sync(IsParameterSet(nameof(Value)), Value, DefaultValue, ValueChanged);
        _values.Sync(IsParameterSet(nameof(Values)), Values ?? [], DefaultValues ?? [], ValuesChanged);
        _context.Type = Type;
        _context.Disabled = Disabled;
        _context.RovingFocus = RovingFocus;
        _context.Orientation = Orientation;
        _context.Focus.Orientation = Orientation;
        _context.Focus.Direction = Direction;
        _context.Focus.Loop = Loop;
        UpdateContext();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddAttribute(1, "role", "group");
        builder.AddMultipleAttributes(2, AdditionalAttributes);
        builder.AddDataOrientation(3, Orientation);
        builder.AddDataDisabled(4, Disabled);

        builder.OpenComponent<CascadingValue<BaseToggleGroupContext>>(5);
        builder.AddComponentParameter(6, "Value", _context);
        builder.AddComponentParameter(7, "ChildContent", ChildContent);
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
            await _value.SetAsync(string.Equals(_value.Value, value, StringComparison.Ordinal) ? null : value);
        }

        UpdateContext();
        StateHasChanged();
    }
}
