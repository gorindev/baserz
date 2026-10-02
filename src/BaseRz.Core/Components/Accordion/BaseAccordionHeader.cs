using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Accordion;

/// <summary>Heading that wraps the trigger (<c>&lt;h3&gt;</c> by default; set <c>As</c> to match the page outline).</summary>
public class BaseAccordionHeader : BaseRzComponent
{
    [CascadingParameter] private BaseAccordionItemContext? Item { get; set; }

    protected override string DefaultElement => "h3";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddDataState(2, Item?.Open == true);
        builder.AddDataDisabled(3, Item?.Disabled == true);
        if (Item is not null)
        {
            builder.AddDataOrientation(4, Item.Root.Orientation);
        }

        builder.AddContent(5, ChildContent);
        builder.CloseElement();
    }
}
