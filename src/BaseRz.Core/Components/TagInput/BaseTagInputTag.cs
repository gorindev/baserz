using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.TagInput;

/// <summary>One tag. <see cref="Value"/> is what <see cref="BaseTagInputTagRemove"/> deletes.</summary>
public class BaseTagInputTag : BaseRzComponent
{
    [Parameter, EditorRequired] public string Value { get; set; } = "";

    protected override string DefaultElement => "span";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.OpenComponent<CascadingValue<string>>(2);
        builder.AddComponentParameter(3, nameof(CascadingValue<string>.Name), "TagValue");
        builder.AddComponentParameter(4, nameof(CascadingValue<string>.Value), Value);
        builder.AddComponentParameter(5, nameof(CascadingValue<string>.ChildContent), ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }
}
