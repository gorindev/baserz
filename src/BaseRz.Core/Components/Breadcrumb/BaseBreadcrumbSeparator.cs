using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Breadcrumb;

/// <summary>Presentational list item between crumbs; renders <c>/</c> when no child content is given.</summary>
public class BaseBreadcrumbSeparator : BaseRzComponent
{
    public const string DefaultSeparator = "/";

    protected override string DefaultElement => "li";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "role", "presentation");
        builder.AddAttribute(3, "aria-hidden", "true");
        if (ChildContent is null)
        {
            builder.AddContent(4, DefaultSeparator);
        }
        else
        {
            builder.AddContent(5, ChildContent);
        }

        builder.CloseElement();
    }
}
