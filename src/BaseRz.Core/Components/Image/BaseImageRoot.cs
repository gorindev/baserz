using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

using ErrorEventArgs = Microsoft.AspNetCore.Components.Web.ErrorEventArgs;

namespace BaseRz.Core.Components.Image;

/// <summary>
/// <c>&lt;img&gt;</c> with loading and error slots. <see cref="LoadingContent"/> renders next to the image until
/// it loads; on error (or a missing <see cref="Src"/>) the image is removed and <see cref="ErrorContent"/> renders.
/// </summary>
public class BaseImageRoot : BaseRzComponentCore
{
    private string? _src;
    private ImageLoadingStatus _status = ImageLoadingStatus.Idle;

    [Parameter] public string? Src { get; set; }

    [Parameter] public string Alt { get; set; } = string.Empty;

    [Parameter] public RenderFragment? LoadingContent { get; set; }

    [Parameter] public RenderFragment? ErrorContent { get; set; }

    [Parameter] public EventCallback<ImageLoadingStatus> OnLoadingStatusChange { get; set; }

    protected override string DefaultElement => "img";

    protected override async Task OnParametersSetAsync()
    {
        if (_status == ImageLoadingStatus.Idle || !string.Equals(_src, Src, StringComparison.Ordinal))
        {
            _src = Src;
            await SetStatusAsync(ImageLoadingStatusExtensions.Initial(Src));
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (_status != ImageLoadingStatus.Error)
        {
            builder.OpenElement(0, Element);
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "src", Src);
            builder.AddAttribute(3, "alt", Alt);
            builder.AddAttribute(4, DataAttributes.DataState, _status.ToDataState());
            builder.AddAttribute(5, "onload", EventCallback.Factory.Create<ProgressEventArgs>(this, _ => SetStatusAsync(ImageLoadingStatus.Loaded)));
            builder.AddAttribute(6, "onerror", EventCallback.Factory.Create<ErrorEventArgs>(this, _ => SetStatusAsync(ImageLoadingStatus.Error)));
            builder.CloseElement();
        }

        if (_status == ImageLoadingStatus.Loading)
        {
            builder.AddContent(7, LoadingContent);
        }
        else if (_status == ImageLoadingStatus.Error)
        {
            builder.AddContent(8, ErrorContent);
        }
    }

    private Task SetStatusAsync(ImageLoadingStatus status)
    {
        if (_status == status)
        {
            return Task.CompletedTask;
        }

        _status = status;
        return OnLoadingStatusChange.InvokeAsync(status);
    }
}
