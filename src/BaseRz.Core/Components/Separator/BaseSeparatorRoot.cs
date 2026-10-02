using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Separator;

/// <summary>
/// <c>role="separator"</c> (or <c>role="none"</c> when <see cref="Decorative"/>). With child content it renders a
/// labeled separator: two empty <c>aria-hidden</c> segments around the label for the caller to style.
/// </summary>
public class BaseSeparatorRoot : BaseRzComponent
{
    [Parameter] public Orientation Orientation { get; set; } = Orientation.Horizontal;

    /// <summary>Purely visual separator; removed from the accessibility tree.</summary>
    [Parameter] public bool Decorative { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "role", Decorative ? "none" : "separator");
        if (!Decorative && Orientation == Orientation.Vertical)
        {
            builder.AddAttribute(3, "aria-orientation", DataAttributes.Of(Orientation));
        }

        builder.AddDataOrientation(4, Orientation);

        if (ChildContent is not null)
        {
            builder.OpenElement(5, "span");
            builder.AddAttribute(6, "aria-hidden", "true");
            builder.CloseElement();

            builder.AddContent(7, ChildContent);

            builder.OpenElement(8, "span");
            builder.AddAttribute(9, "aria-hidden", "true");
            builder.CloseElement();
        }

        builder.CloseElement();
    }
}
