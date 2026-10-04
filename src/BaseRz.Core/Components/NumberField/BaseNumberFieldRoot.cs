using System.Globalization;
using System.Linq.Expressions;

using BaseRz.Core.Components.Form;
using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.NumberField;

/// <summary>Spinbutton value owner. Arrow keys, Home, and End are handled by <see cref="BaseNumberFieldInput"/>.</summary>
public class BaseNumberFieldRoot : BaseRzComponent
{
    private readonly ControllableState<decimal?> _value = new();
    private BaseNumberFieldContext _context = default!;

    [CascadingParameter] private EditContext? EditContext { get; set; }

    [CascadingParameter] private BaseFieldContext? Field { get; set; }

    [Parameter] public decimal? Value { get; set; }

    [Parameter] public EventCallback<decimal?> ValueChanged { get; set; }

    [Parameter] public Expression<Func<decimal?>>? ValueExpression { get; set; }

    [Parameter] public decimal? DefaultValue { get; set; }

    [Parameter] public decimal Min { get; set; }

    [Parameter] public decimal Max { get; set; } = 100;

    [Parameter] public decimal Step { get; set; } = 1;

    [Parameter] public bool Disabled { get; set; }

    [Parameter] public bool ReadOnly { get; set; }

    [Parameter] public bool Required { get; set; }

    [Parameter] public string? Name { get; set; }

    protected override void OnInitialized()
    {
        _context = new BaseNumberFieldContext(SetAsync, StepByAsync);
    }

    protected override void OnParametersSet()
    {
        _value.Sync(IsParameterSet(nameof(Value)), Value, DefaultValue, ValueChanged);
        _context.Value = _value.Value;
        _context.Min = Min;
        _context.Max = Max;
        _context.Step = Step == 0 ? 1 : Step;
        _context.Disabled = Disabled;
        _context.ReadOnly = ReadOnly;
        _context.Required = Required || (Field?.Required ?? false);
        _context.Name = Name;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddDataDisabled(2, Disabled);

        builder.OpenComponent<CascadingValue<BaseNumberFieldContext>>(3);
        builder.AddComponentParameter(4, nameof(CascadingValue<BaseNumberFieldContext>.Value), _context);
        builder.AddComponentParameter(5, nameof(CascadingValue<BaseNumberFieldContext>.ChildContent), ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }

    internal async Task SetAsync(decimal value)
    {
        if (Disabled || ReadOnly)
        {
            return;
        }

        var next = Clamp(value);
        await _value.SetAsync(next);
        _context.Value = _value.Value;
        Notify();
        StateHasChanged();
    }

    internal async Task StepByAsync(int direction)
    {
        var start = _value.Value ?? Min;
        await SetAsync(start + (_context.Step * direction));
    }

    internal decimal Clamp(decimal value)
    {
        if (value < Min)
        {
            value = Min;
        }

        if (value > Max)
        {
            value = Max;
        }

        var step = Step == 0 ? 1 : Step;
        var steps = Math.Round((value - Min) / step, MidpointRounding.AwayFromZero);
        value = Min + (steps * step);
        if (value > Max)
        {
            value = Max;
        }

        if (value < Min)
        {
            value = Min;
        }

        return value;
    }

    internal static string Format(decimal value) => value.ToString(CultureInfo.InvariantCulture);

    private void Notify()
    {
        if (EditContext is null)
        {
            return;
        }

        if (ValueExpression is not null)
        {
            EditContext.NotifyFieldChanged(FieldIdentifier.Create(ValueExpression));
        }
        else
        {
            Field?.NotifyEditContext();
        }
    }
}
