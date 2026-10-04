using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.SegmentedInput;

/// <summary>Visual separator between slots. Hidden from assistive technology.</summary>
public class BaseSegmentedInputSeparator : BaseRzComponent
{
    protected override string DefaultElement => "span";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "aria-hidden", "true");
        if (ChildContent is null)
        {
            builder.AddContent(3, "-");
        }
        else
        {
            builder.AddContent(3, ChildContent);
        }

        builder.CloseElement();
    }
}
