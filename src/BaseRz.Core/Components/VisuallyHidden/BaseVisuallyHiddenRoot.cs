using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.VisuallyHidden;

/// <summary>
/// Hides content visually while keeping it in the accessibility tree. The clip style is emitted inline because
/// BaseRz never emits its own classes and the reset stylesheet is opt-in.
/// </summary>
public class BaseVisuallyHiddenRoot : BaseRzComponent
{
    public const string HiddenStyle =
        "position: absolute; border: 0; width: 1px; height: 1px; padding: 0; margin: -1px; overflow: hidden; "
        + "clip: rect(0, 0, 0, 0); white-space: nowrap; word-wrap: normal";

    protected override string DefaultElement => "span";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, InlineStyle.AttributesWithoutStyle(AdditionalAttributes));
        builder.AddAttribute(2, "style", InlineStyle.Merge(AdditionalAttributes, HiddenStyle));
        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }
}
