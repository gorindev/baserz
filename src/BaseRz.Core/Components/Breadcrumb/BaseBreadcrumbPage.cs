using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Breadcrumb;

/// <summary>The current page: not a link, marked with <c>aria-current="page"</c>.</summary>
public class BaseBreadcrumbPage : BaseRzComponent
{
    protected override string DefaultElement => "span";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "role", "link");
        builder.AddAttribute(3, "aria-disabled", "true");
        builder.AddAttribute(4, "aria-current", "page");
        builder.AddContent(5, ChildContent);
        builder.CloseElement();
    }
}
