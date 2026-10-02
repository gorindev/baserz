using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.RadioGroup;

/// <summary>Rendered only while its radio is checked, so callers can style the dot.</summary>
public class BaseRadioGroupIndicator : BaseRzComponent
{
    [CascadingParameter] private BaseRadioGroupItemState? Item { get; set; }

    protected override string DefaultElement => "span";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Item is not { Checked: true })
        {
            return;
        }

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, DataAttributes.DataState, BaseRadioGroupItem.Checked);
        builder.AddDataDisabled(3, Item.Disabled);
        builder.AddContent(4, ChildContent);
        builder.CloseElement();
    }
}
