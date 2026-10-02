using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

using ErrorEventArgs = Microsoft.AspNetCore.Components.Web.ErrorEventArgs;

namespace BaseRz.Core.Components.Avatar;

/// <summary>
/// The avatar <c>&lt;img&gt;</c>. Rendered while loading (with <c>data-state="loading"</c>, so callers can hide it)
/// and once loaded; removed on error so <see cref="BaseAvatarFallback"/> takes over. A new <see cref="Src"/> restarts loading.
/// </summary>
public class BaseAvatarImage : BaseRzComponentCore, IDisposable
{
    private string? _src;
    private ImageLoadingStatus _status = ImageLoadingStatus.Idle;

    [CascadingParameter] private BaseAvatarContext? Context { get; set; }

    [Parameter] public string? Src { get; set; }

    [Parameter] public string Alt { get; set; } = string.Empty;

    protected override string DefaultElement => "img";

    protected override void OnParametersSet()
    {
        if (_status == ImageLoadingStatus.Idle || !string.Equals(_src, Src, StringComparison.Ordinal))
        {
            _src = Src;
            SetStatus(ImageLoadingStatusExtensions.Initial(Src));
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (_status == ImageLoadingStatus.Error)
        {
            return;
        }

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "src", Src);
        builder.AddAttribute(3, "alt", Alt);
        builder.AddAttribute(4, DataAttributes.DataState, _status.ToDataState());
        builder.AddAttribute(5, "onload", EventCallback.Factory.Create<ProgressEventArgs>(this, _ => SetStatus(ImageLoadingStatus.Loaded)));
        builder.AddAttribute(6, "onerror", EventCallback.Factory.Create<ErrorEventArgs>(this, _ => SetStatus(ImageLoadingStatus.Error)));
        builder.CloseElement();
    }

    private void SetStatus(ImageLoadingStatus status)
    {
        _status = status;
        Context?.SetStatus(status);
    }

    public void Dispose()
    {
        Context?.SetStatus(ImageLoadingStatus.Idle);
        GC.SuppressFinalize(this);
    }
}
