using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Tabs;

/// <summary><c>role="tablist"</c> with <c>aria-orientation</c> from the root.</summary>
public class BaseTabsList : BaseRzComponent
{
    [CascadingParameter] private BaseTabsContext? Context { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var orientation = Context?.Orientation ?? Orientation.Horizontal;

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "role", "tablist");
        builder.AddAttribute(3, "aria-orientation", DataAttributes.Of(orientation));
        builder.AddDataOrientation(4, orientation);
        builder.AddContent(5, ChildContent);
        builder.CloseElement();
    }
}
