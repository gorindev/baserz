using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Breadcrumb;

/// <summary>Landmark <c>&lt;nav&gt;</c> labeled "Breadcrumb" unless the caller passes its own <c>aria-label</c>.</summary>
public class BaseBreadcrumbRoot : BaseRzComponent
{
    public const string DefaultLabel = "Breadcrumb";

    protected override string DefaultElement => "nav";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        if (!HasAttribute("aria-label") && !HasAttribute("aria-labelledby"))
        {
            builder.AddAttribute(1, "aria-label", DefaultLabel);
        }

        builder.AddMultipleAttributes(2, AdditionalAttributes);
        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }
}
