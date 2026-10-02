using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Tabs;

/// <summary><c>role="tabpanel"</c> for <see cref="Value"/>, labelled by its tab and <c>hidden</c> while inactive.</summary>
public class BaseTabsContent : BaseRzComponent
{
    [CascadingParameter] private BaseTabsContext? Context { get; set; }

    [Parameter, EditorRequired] public string Value { get; set; } = string.Empty;

    protected override void OnParametersSet() => Context?.UseContentId(Value, AdditionalAttributes);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var selected = Context?.IsSelected(Value) == true;

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "role", "tabpanel");
        builder.AddAttribute(3, "id", Context?.ContentId(Value));
        builder.AddAttribute(4, "aria-labelledby", Context?.TriggerId(Value));
        builder.AddAttribute(5, "tabindex", "0");
        builder.AddAttribute(6, DataAttributes.DataState, selected ? BaseTabsTrigger.Active : BaseTabsTrigger.Inactive);
        if (Context is not null)
        {
            builder.AddDataOrientation(7, Context.Orientation);
        }

        if (!selected)
        {
            builder.AddAttribute(8, "hidden", true);
        }

        builder.AddContent(9, ChildContent);
        builder.CloseElement();
    }
}
