using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Accordion;

/// <summary><c>role="region"</c> labelled by the item's trigger; <c>hidden</c> and <c>inert</c> while closed.</summary>
public class BaseAccordionContent : BaseRzComponent
{
    [CascadingParameter] private BaseAccordionItemContext? Item { get; set; }

    protected override void OnParametersSet() => Item?.ContentId.Use(AdditionalAttributes);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var open = Item?.Open == true;

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "id", Item?.ContentId.Value);
        builder.AddAttribute(3, "role", "region");
        builder.AddAttribute(4, "aria-labelledby", Item?.TriggerId.Value);
        builder.AddDataState(5, open);
        if (Item is not null)
        {
            builder.AddDataOrientation(6, Item.Root.Orientation);
        }

        builder.AddDisclosureHidden(7, open);
        builder.AddContent(9, ChildContent);
        builder.CloseElement();
    }
}
