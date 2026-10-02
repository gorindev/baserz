using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Alert;

/// <summary>
/// <c>role="alert"</c> container. A mounted <see cref="BaseAlertTitle"/> / <see cref="BaseAlertDescription"/>
/// is referenced through <c>aria-labelledby</c> / <c>aria-describedby</c>.
/// </summary>
public class BaseAlertRoot : BaseRzComponent
{
    private BaseAlertContext _context = default!;

    protected override string IdPrefix => "baserz-alert";

    protected override void OnInitialized()
    {
        _context = new BaseAlertContext(() => InvokeAsync(StateHasChanged));
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "role", "alert");
        if (_context.TitleId is not null)
        {
            builder.AddAttribute(3, "aria-labelledby", _context.TitleId);
        }

        if (_context.DescriptionId is not null)
        {
            builder.AddAttribute(4, "aria-describedby", _context.DescriptionId);
        }

        builder.OpenComponent<CascadingValue<BaseAlertContext>>(5);
        builder.AddComponentParameter(6, "Value", _context);
        builder.AddComponentParameter(7, "IsFixed", true);
        builder.AddComponentParameter(8, "ChildContent", ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }
}
