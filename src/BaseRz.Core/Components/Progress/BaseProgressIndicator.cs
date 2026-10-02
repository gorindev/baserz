using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Progress;

/// <summary>
/// Visual fill. Carries the same <c>data-state</c> / <c>data-value</c> / <c>data-max</c> as the root; size it
/// yourself (for example from <see cref="BaseProgressContext.Percentage"/>).
/// </summary>
public class BaseProgressIndicator : BaseRzComponent
{
    [CascadingParameter] private BaseProgressContext? Context { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        Context?.AddDataAttributes(builder, 2);
        builder.AddContent(5, ChildContent);
        builder.CloseElement();
    }
}
