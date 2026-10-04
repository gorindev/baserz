using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Slider;

/// <summary>Pointer target for the slider. Presentational; the thumb carries <c>role="slider"</c>.</summary>
public class BaseSliderTrack : BaseRzComponent
{
    private ElementReference _element;

    [CascadingParameter] private BaseSliderContext? Context { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "aria-hidden", "true");
        builder.AddElementReferenceCapture(3, element => _element = element);
        builder.AddContent(4, ChildContent);
        builder.CloseElement();
    }

    private bool _attached;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_attached && Context is not null)
        {
            _attached = true;
            await Context.AttachTrackAsync(_element);
        }
    }
}
