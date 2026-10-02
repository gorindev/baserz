using System.Globalization;

using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.Pagination;

/// <summary>
/// Goes to <see cref="Page"/>. The current page gets <c>aria-current="page"</c> and <c>data-state="active"</c>.
/// Renders the page number when no child content is given.
/// </summary>
public class BasePaginationLink : BaseRzComponent
{
    public const string Active = "active";
    public const string Inactive = "inactive";

    [CascadingParameter] private BasePaginationContext? Context { get; set; }

    [Parameter] public int Page { get; set; }

    protected override string DefaultElement => "button";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var active = Context?.Page == Page;
        var disabled = Context?.Disabled == true;

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        if (ButtonSemantics.IsNative(Element))
        {
            builder.AddAttribute(2, "type", "button");
            builder.AddAttribute(3, "disabled", disabled);
        }

        if (active)
        {
            builder.AddAttribute(4, "aria-current", "page");
        }

        builder.AddAttribute(5, DataAttributes.DataState, active ? Active : Inactive);
        builder.AddDataDisabled(6, disabled);
        builder.AddAttribute(7, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, GoAsync));
        if (ChildContent is null)
        {
            builder.AddContent(8, Page.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            builder.AddContent(9, ChildContent);
        }

        builder.CloseElement();
    }

    private Task GoAsync() => Context?.SetPageAsync(Page) ?? Task.CompletedTask;
}
