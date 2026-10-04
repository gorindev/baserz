using BaseRz.Core.Components.Form;
using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.NumberField;

/// <summary><c>role="spinbutton"</c>. Up/Down change by step; Home/End jump to min/max.</summary>
public class BaseNumberFieldInput : BaseRzComponent
{
    [CascadingParameter] private BaseNumberFieldContext? Context { get; set; }

    [CascadingParameter] private BaseFieldContext? Field { get; set; }

    protected override string DefaultElement => "input";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var disabled = Context?.Disabled == true;
        var value = Context?.Value;

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "role", "spinbutton");
        builder.AddAttribute(3, "inputmode", "decimal");
        builder.AddAttribute(4, "value", value is null ? "" : BaseNumberFieldRoot.Format(value.Value));
        builder.AddAttribute(5, "aria-valuemin", Context is null ? null : BaseNumberFieldRoot.Format(Context.Min));
        builder.AddAttribute(6, "aria-valuemax", Context is null ? null : BaseNumberFieldRoot.Format(Context.Max));
        if (value is not null && Context is not null)
        {
            builder.AddAttribute(7, "aria-valuenow", BaseNumberFieldRoot.Format(value.Value));
        }

        if (Context?.Name is not null)
        {
            builder.AddAttribute(8, "name", Context.Name);
        }

        if (disabled)
        {
            builder.AddAttribute(9, "disabled", true);
        }

        if (Context?.ReadOnly == true)
        {
            builder.AddAttribute(10, "readonly", true);
        }

        if (Context?.Required == true && !HasAttribute("aria-required"))
        {
            builder.AddAttribute(11, "aria-required", "true");
        }

        builder.AddAttribute(12, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, OnInputAsync));
        builder.AddAttribute(13, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, OnKeyDownAsync));
        builder.SetUpdatesAttributeName("value");
        FieldAria.Apply(builder, 14, Field, HasAttribute, labelledBy: false);
        builder.AddDataDisabled(20, disabled);
        builder.CloseElement();
    }

    private async Task OnInputAsync(ChangeEventArgs args)
    {
        if (Context is null || Context.Disabled || Context.ReadOnly)
        {
            return;
        }

        var text = args.Value?.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        if (decimal.TryParse(text, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
        {
            await Context.SetAsync(parsed);
        }
    }

    private Task OnKeyDownAsync(KeyboardEventArgs args)
    {
        if (Context is null || Context.Disabled || Context.ReadOnly)
        {
            return Task.CompletedTask;
        }

        return args.Key switch
        {
            "ArrowUp" => Context.StepAsync(1),
            "ArrowDown" => Context.StepAsync(-1),
            "Home" => Context.SetAsync(Context.Min),
            "End" => Context.SetAsync(Context.Max),
            _ => Task.CompletedTask,
        };
    }
}
