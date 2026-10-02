using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.CheckboxGroup;

/// <summary>Help text for the group, referenced by the root's <c>aria-describedby</c> while mounted.</summary>
public class BaseCheckboxGroupDescription : BaseRzComponent, IDisposable
{
    [CascadingParameter] private BaseCheckboxGroupContext? Context { get; set; }

    protected override string IdPrefix => "baserz-checkbox-group-description";

    protected override void OnParametersSet() => Context?.DescriptionId.Set(this, Id);

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
        Context?.DescriptionId.Clear(this);
        GC.SuppressFinalize(this);
    }
}
