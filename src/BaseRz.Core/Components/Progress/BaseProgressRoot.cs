using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Progress;

/// <summary>
/// <c>role="progressbar"</c>. A <see langword="null"/> <see cref="Value"/> is indeterminate and omits
/// <c>aria-valuenow</c>; otherwise the value is clamped to <see cref="Min"/>..<see cref="Max"/>.
/// </summary>
public class BaseProgressRoot : BaseRzComponent
{
    private readonly BaseProgressContext _context = new();

    [Parameter] public double? Value { get; set; }

    [Parameter] public double Min { get; set; }

    [Parameter] public double Max { get; set; } = 100;

    /// <summary>Human-readable value for <c>aria-valuetext</c>, given <c>(value, max)</c>.</summary>
    [Parameter] public Func<double, double, string>? GetValueLabel { get; set; }

    protected override void OnParametersSet()
    {
        var max = Max > Min ? Max : Min + 100;
        _context.Min = Min;
        _context.Max = max;
        _context.Value = Value is { } value && !double.IsNaN(value) ? Math.Clamp(value, Min, max) : null;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "role", "progressbar");
        builder.AddAttribute(3, "aria-valuemin", BaseProgressContext.Format(_context.Min));
        builder.AddAttribute(4, "aria-valuemax", BaseProgressContext.Format(_context.Max));
        if (_context.Value is { } value)
        {
            builder.AddAttribute(5, "aria-valuenow", BaseProgressContext.Format(value));
            if (GetValueLabel is not null)
            {
                builder.AddAttribute(6, "aria-valuetext", GetValueLabel(value, _context.Max));
            }
        }

        _context.AddDataAttributes(builder, 7);

        builder.OpenComponent<CascadingValue<BaseProgressContext>>(10);
        builder.AddComponentParameter(11, "Value", _context);
        builder.AddComponentParameter(12, "ChildContent", ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }
}
