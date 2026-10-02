using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.CheckboxGroup;

/// <summary>
/// <c>role="group"</c> of <c>BaseCheckboxRoot</c>s whose <c>Value</c>s make up a collection (<c>@bind-Values</c>
/// or <see cref="DefaultValues"/>). A mounted Label names the group; Description and ErrorMessage are referenced
/// by <c>aria-describedby</c>, and an ErrorMessage also sets <c>aria-invalid</c>.
/// </summary>
public class BaseCheckboxGroupRoot : BaseRzComponent
{
    private readonly ControllableState<IReadOnlyCollection<string>> _values = new(ValueSet.Comparer);
    private BaseCheckboxGroupContext _context = default!;

    [Parameter] public IReadOnlyCollection<string>? Values { get; set; }

    [Parameter] public EventCallback<IReadOnlyCollection<string>> ValuesChanged { get; set; }

    [Parameter] public IReadOnlyCollection<string>? DefaultValues { get; set; }

    [Parameter] public bool Disabled { get; set; }

    /// <summary>Form field name for the checkboxes' hidden inputs.</summary>
    [Parameter] public string? Name { get; set; }

    protected override void OnInitialized()
    {
        _context = new BaseCheckboxGroupContext(() => InvokeAsync(StateHasChanged), SetAsync);
    }

    protected override void OnParametersSet()
    {
        _values.Sync(IsParameterSet(nameof(Values)), Values ?? [], DefaultValues ?? [], ValuesChanged);
        _context.Values = _values.Value ?? [];
        _context.Disabled = Disabled;
        _context.Name = Name;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var describedBy = string.Join(' ', new[]
        {
            AdditionalAttributes?.GetValueOrDefault("aria-describedby") as string,
            _context.DescriptionId.Value,
            _context.ErrorMessageId.Value,
        }.Where(static id => !string.IsNullOrWhiteSpace(id)));

        builder.OpenElement(0, Element);
        builder.AddAttribute(1, "role", "group");
        if (_context.LabelId.Value is { } labelId && !HasAttribute("aria-label"))
        {
            builder.AddAttribute(2, "aria-labelledby", labelId);
        }

        builder.AddMultipleAttributes(3, AdditionalAttributes);
        if (describedBy.Length > 0)
        {
            builder.AddAttribute(4, "aria-describedby", describedBy);
        }

        if (_context.ErrorMessageId.Value is not null)
        {
            builder.AddAttribute(5, "aria-invalid", "true");
        }

        builder.AddDataDisabled(6, Disabled);

        builder.OpenComponent<CascadingValue<BaseCheckboxGroupContext>>(7);
        builder.AddComponentParameter(8, "Value", _context);
        builder.AddComponentParameter(9, "ChildContent", ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }

    private async Task SetAsync(string value, bool isChecked)
    {
        if (Disabled)
        {
            return;
        }

        await _values.SetAsync(ValueSet.With(_values.Value, value, isChecked));
        _context.Values = _values.Value ?? [];
        StateHasChanged();
    }
}
