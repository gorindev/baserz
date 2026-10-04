using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.TagInput;

/// <summary>Removes the surrounding tag. Accessible name defaults to "Remove {value}".</summary>
public class BaseTagInputTagRemove : BaseRzComponent
{
    [CascadingParameter] private BaseTagInputContext? Context { get; set; }

    [CascadingParameter(Name = "TagValue")] private string? Value { get; set; }

    protected override string DefaultElement => "button";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "type", "button");
        if (!HasAttribute("aria-label"))
        {
            builder.AddAttribute(3, "aria-label", $"Remove {Value}");
        }

        builder.AddAttribute(4, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () =>
            Value is null ? Task.CompletedTask : Context?.RemoveAsync(Value) ?? Task.CompletedTask));
        builder.AddContent(5, ChildContent);
        builder.CloseElement();
    }
}
