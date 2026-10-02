using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Label;

/// <summary>Native <c>&lt;label&gt;</c>; <see cref="For"/> associates it with the control of that id.</summary>
public class BaseLabelRoot : BaseRzComponent
{
    [Parameter] public string? For { get; set; }

    protected override string DefaultElement => "label";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        if (!string.IsNullOrWhiteSpace(For))
        {
            builder.AddAttribute(2, "for", For);
        }

        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }
}
