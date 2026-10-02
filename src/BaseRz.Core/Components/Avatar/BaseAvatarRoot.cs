using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Avatar;

/// <summary>
/// Avatar container. <c>data-state</c> mirrors the image status: <c>idle</c> (no image part), <c>loading</c>,
/// <c>loaded</c>, or <c>error</c>.
/// </summary>
public class BaseAvatarRoot : BaseRzComponent
{
    private BaseAvatarContext _context = default!;

    protected override string DefaultElement => "span";

    protected override void OnInitialized()
    {
        _context = new BaseAvatarContext(() => InvokeAsync(StateHasChanged));
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, DataAttributes.DataState, _context.Status.ToDataState());

        builder.OpenComponent<CascadingValue<BaseAvatarContext>>(3);
        builder.AddComponentParameter(4, "Value", _context);
        builder.AddComponentParameter(5, "ChildContent", ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }
}
