using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.NumberField;

/// <summary>Groups the spinbutton with its increment and decrement buttons.</summary>
public class BaseNumberFieldGroup : BaseRzComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddContent(2, ChildContent);
        builder.CloseElement();
    }
}
