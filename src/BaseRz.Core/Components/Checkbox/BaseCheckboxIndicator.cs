using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Checkbox;

/// <summary>Rendered only while the checkbox is checked or indeterminate; carries the same <c>data-state</c>.</summary>
public class BaseCheckboxIndicator : BaseRzComponent
{
    [CascadingParameter] private BaseCheckboxState? Checkbox { get; set; }

    protected override string DefaultElement => "span";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Checkbox is null || Checkbox.State == BaseCheckboxRoot.UncheckedState)
        {
            return;
        }

        builder.OpenElement(0, Element);
        builder.AddAttribute(1, "aria-hidden", "true");
        builder.AddMultipleAttributes(2, AdditionalAttributes);
        builder.AddAttribute(3, DataAttributes.DataState, Checkbox.State);
        builder.AddDataDisabled(4, Checkbox.Disabled);
        builder.AddContent(5, ChildContent);
        builder.CloseElement();
    }
}
