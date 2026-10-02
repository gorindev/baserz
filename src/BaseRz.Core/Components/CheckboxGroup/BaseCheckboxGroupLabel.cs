using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.CheckboxGroup;

/// <summary>Visible name of the group, referenced by the root's <c>aria-labelledby</c> while mounted.</summary>
public class BaseCheckboxGroupLabel : BaseRzComponent, IDisposable
{
    [CascadingParameter] private BaseCheckboxGroupContext? Context { get; set; }

    protected override string DefaultElement => "span";

    protected override string IdPrefix => "baserz-checkbox-group-label";

    protected override void OnParametersSet() => Context?.LabelId.Set(this, Id);

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
        Context?.LabelId.Clear(this);
        GC.SuppressFinalize(this);
    }
}
