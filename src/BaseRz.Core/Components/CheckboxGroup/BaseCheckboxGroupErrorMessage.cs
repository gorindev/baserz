using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.CheckboxGroup;

/// <summary>
/// <c>role="alert"</c> validation message. While mounted it is added to the root's <c>aria-describedby</c> and
/// the root gets <c>aria-invalid="true"</c>; mount it only when the group is invalid.
/// </summary>
public class BaseCheckboxGroupErrorMessage : BaseRzComponent, IDisposable
{
    [CascadingParameter] private BaseCheckboxGroupContext? Context { get; set; }

    protected override string IdPrefix => "baserz-checkbox-group-error";

    protected override void OnParametersSet() => Context?.ErrorMessageId.Set(this, Id);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddAttribute(1, "role", "alert");
        builder.AddMultipleAttributes(2, AdditionalAttributes);
        builder.AddAttribute(3, "id", Id);
        builder.AddContent(4, ChildContent);
        builder.CloseElement();
    }

    public void Dispose()
    {
        Context?.ErrorMessageId.Clear(this);
        GC.SuppressFinalize(this);
    }
}
