using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Collapsible;

/// <summary>
/// <c>role="region"</c> labelled by the trigger. Stays in the DOM when closed but is <c>hidden</c> and <c>inert</c>,
/// so callers can animate on <c>data-state</c> by overriding <c>[hidden]</c>.
/// </summary>
public class BaseCollapsibleContent : BaseRzComponent
{
    [CascadingParameter] private BaseCollapsibleContext? Context { get; set; }

    protected override void OnParametersSet() => Context?.ContentId.Use(AdditionalAttributes);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var open = Context?.Open == true;

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "id", Context?.ContentId.Value);
        builder.AddAttribute(3, "role", "region");
        builder.AddAttribute(4, "aria-labelledby", Context?.TriggerId.Value);
        builder.AddDataState(5, open);
        builder.AddDisclosureHidden(6, open);
        builder.AddContent(8, ChildContent);
        builder.CloseElement();
    }
}
