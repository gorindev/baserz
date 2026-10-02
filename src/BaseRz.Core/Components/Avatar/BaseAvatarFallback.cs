using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Avatar;

/// <summary>
/// Shown until the image has loaded. While the image is still loading it is <c>aria-hidden</c> so the image's
/// <c>alt</c> is the only accessible name; once the image loads it is removed.
/// </summary>
public class BaseAvatarFallback : BaseRzComponent
{
    [CascadingParameter] private BaseAvatarContext? Context { get; set; }

    protected override string DefaultElement => "span";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var status = Context?.Status ?? ImageLoadingStatus.Idle;
        if (status == ImageLoadingStatus.Loaded)
        {
            return;
        }

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        if (status == ImageLoadingStatus.Loading)
        {
            builder.AddAttribute(2, "aria-hidden", "true");
        }

        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }
}
