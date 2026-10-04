using System.Linq.Expressions;

using BaseRz.Core.Components.Form;
using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Input;

/// <summary>
/// Native <c>&lt;input&gt;</c>. Inside a <see cref="BaseField{TValue}"/> it uses the field id and
/// <c>aria-invalid</c> / <c>aria-describedby</c> / <c>aria-required</c>, and notifies <see cref="EditContext"/>.
/// </summary>
public class BaseInputRoot : BaseRzComponent
{
    private readonly ControllableState<string?> _value = new();

    [CascadingParameter] private EditContext? EditContext { get; set; }

    [CascadingParameter] private BaseFieldContext? Field { get; set; }

    [Parameter] public string? Value { get; set; }

    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    [Parameter] public Expression<Func<string?>>? ValueExpression { get; set; }

    [Parameter] public string? DefaultValue { get; set; }

    [Parameter] public string Type { get; set; } = "text";

    [Parameter] public bool Disabled { get; set; }

    [Parameter] public bool ReadOnly { get; set; }

    [Parameter] public string? Name { get; set; }

    [Parameter] public string? Placeholder { get; set; }

    protected override string DefaultElement => "input";

    protected override void OnParametersSet()
    {
        _value.Sync(IsParameterSet(nameof(Value)), Value, DefaultValue, ValueChanged);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "type", string.IsNullOrWhiteSpace(Type) ? "text" : Type);
        builder.AddAttribute(3, "value", _value.Value);
        builder.AddAttribute(4, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleInputAsync));
        builder.SetUpdatesAttributeName("value");

        if (!string.IsNullOrWhiteSpace(Name))
        {
            builder.AddAttribute(5, "name", Name);
        }

        if (!string.IsNullOrWhiteSpace(Placeholder))
        {
            builder.AddAttribute(6, "placeholder", Placeholder);
        }

        if (!HasAttribute("id") && Field is not null)
        {
            builder.AddAttribute(7, "id", Field.Id);
        }

        if (Disabled)
        {
            builder.AddAttribute(8, "disabled", true);
        }

        if (ReadOnly)
        {
            builder.AddAttribute(9, "readonly", true);
        }

        if (Field is not null)
        {
            if (!HasAttribute("aria-invalid") && Field.HasErrors)
            {
                builder.AddAttribute(10, "aria-invalid", "true");
            }

            if (!HasAttribute("aria-describedby") && Field.DescribedBy is not null)
            {
                builder.AddAttribute(11, "aria-describedby", Field.DescribedBy);
            }

            if (!HasAttribute("aria-required") && Field.Required)
            {
                builder.AddAttribute(12, "aria-required", "true");
            }
        }

        builder.AddDataDisabled(13, Disabled);
        builder.CloseElement();
    }

    private async Task HandleInputAsync(ChangeEventArgs args)
    {
        if (Disabled || ReadOnly)
        {
            return;
        }

        var next = args.Value?.ToString();
        await _value.SetAsync(next);
        Notify();
    }

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
