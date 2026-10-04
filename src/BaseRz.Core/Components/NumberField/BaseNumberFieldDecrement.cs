using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.NumberField;

/// <summary>Button that subtracts one step.</summary>
public class BaseNumberFieldDecrement : BaseRzComponent
{
    [CascadingParameter] private BaseNumberFieldContext? Context { get; set; }

    protected override string DefaultElement => "button";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var disabled = Context?.Disabled == true || Context?.ReadOnly == true;
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddButtonAttributes(2, ButtonSemantics.IsNative(Element), "button", disabled);
        if (!HasAttribute("aria-label"))
        {
            builder.AddAttribute(8, "aria-label", "Decrement");
        }

        builder.AddAttribute(9, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => Context?.StepAsync(-1) ?? Task.CompletedTask));
        builder.AddDataDisabled(10, disabled);
        builder.AddContent(11, ChildContent);
        builder.CloseElement();
    }
}
