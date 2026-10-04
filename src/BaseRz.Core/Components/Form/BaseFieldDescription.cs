using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Form;

/// <summary>Help text. While mounted its id is included in the field's <c>aria-describedby</c>.</summary>
public class BaseFieldDescription : BaseRzComponent, IDisposable
{
    [CascadingParameter] private BaseFieldContext? Context { get; set; }

    protected override string DefaultElement => "p";

    private string DescriptionId => HasAttribute("id") || Context is null ? Id : $"{Context.Id}-description";

    protected override void OnParametersSet() => Context?.DescriptionId.Set(this, DescriptionId);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "id", DescriptionId);
        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }

    public void Dispose()
    {
        Context?.DescriptionId.Clear(this);
        GC.SuppressFinalize(this);
    }
}
