using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Alert;

/// <summary>Heading for the alert (<c>&lt;h5&gt;</c> by default; set <c>As</c> to match the page outline).</summary>
public class BaseAlertTitle : BaseRzComponent, IDisposable
{
    [CascadingParameter] private BaseAlertContext? Context { get; set; }

    protected override string DefaultElement => "h5";

    protected override string IdPrefix => "baserz-alert-title";

    protected override void OnParametersSet() => Context?.SetTitle(this, Id);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "id", Id);
        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }

    public void Dispose()
    {
        Context?.ClearTitle(this);
        GC.SuppressFinalize(this);
    }
}
