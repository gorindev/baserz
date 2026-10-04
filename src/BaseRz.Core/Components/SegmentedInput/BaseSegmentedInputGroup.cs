using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.SegmentedInput;

/// <summary>Groups the slots.</summary>
public class BaseSegmentedInputGroup : BaseRzComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddAttribute(1, "role", "group");
        builder.AddMultipleAttributes(2, AdditionalAttributes);
        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }
}
