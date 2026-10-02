using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Pagination;

/// <summary>Hidden-from-AT marker for skipped pages; renders <c>…</c> when no child content is given.</summary>
public class BasePaginationEllipsis : BaseRzComponent
{
    public const string DefaultText = "…";

    protected override string DefaultElement => "span";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "aria-hidden", "true");
        if (ChildContent is null)
        {
            builder.AddContent(3, DefaultText);
        }
        else
        {
            builder.AddContent(4, ChildContent);
        }

        builder.CloseElement();
    }
}
