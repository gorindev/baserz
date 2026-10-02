using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components;

/// <summary>A part that is only its element: attributes and child content, no state or ARIA of its own.</summary>
public abstract class BaseRzElementComponent : BaseRzComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddContent(2, ChildContent);
        builder.CloseElement();
    }
}
