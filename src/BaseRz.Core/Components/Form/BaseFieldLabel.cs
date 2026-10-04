using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Form;

/// <summary>Label for the field control. <c>for</c> is the field id.</summary>
public class BaseFieldLabel : BaseRzComponent, IDisposable
{
    [CascadingParameter] private BaseFieldContext? Context { get; set; }

    protected override string DefaultElement => "label";

    protected override string IdPrefix => "baserz-field-label";

    protected override void OnParametersSet() => Context?.LabelId.Set(this, Id);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "id", Id);
        if (Context is not null)
        {
            builder.AddAttribute(3, "for", Context.Id);
        }

        builder.AddContent(4, ChildContent);
        builder.CloseElement();
    }

    public void Dispose()
    {
        Context?.LabelId.Clear(this);
        GC.SuppressFinalize(this);
    }
}
