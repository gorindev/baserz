using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Alert;

public class BaseAlertDescription : BaseRzComponent, IDisposable
{
    [CascadingParameter] private BaseAlertContext? Context { get; set; }

    protected override string IdPrefix => "baserz-alert-description";

    protected override void OnParametersSet() => Context?.SetDescription(this, Id);

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
        Context?.ClearDescription(this);
        GC.SuppressFinalize(this);
    }
}
