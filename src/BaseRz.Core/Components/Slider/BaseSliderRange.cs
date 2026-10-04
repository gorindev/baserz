using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Slider;

/// <summary>Filled portion of the track. Size is an inline style.</summary>
public class BaseSliderRange : BaseRzComponent
{
    [CascadingParameter] private BaseSliderContext? Context { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "aria-hidden", "true");
        if (Context?.RangeStyle is not null)
        {
            builder.AddAttribute(3, "style", Context.RangeStyle);
        }

        builder.AddContent(4, ChildContent);
        builder.CloseElement();
    }
}
