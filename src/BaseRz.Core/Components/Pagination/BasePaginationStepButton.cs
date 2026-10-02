using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.Pagination;

/// <summary>Shared behavior for <see cref="BasePaginationPrev"/> and <see cref="BasePaginationNext"/>.</summary>
public abstract class BasePaginationStepButton : BaseRzComponent
{
    [CascadingParameter] private BasePaginationContext? Context { get; set; }

    protected override string DefaultElement => "button";

    protected abstract string DefaultLabel { get; }

    protected abstract string DefaultText { get; }

    protected abstract int Step { get; }

    protected abstract bool CanStep(BasePaginationContext context);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var disabled = Context is null || !CanStep(Context);

        builder.OpenElement(0, Element);
        if (!HasAttribute("aria-label") && !HasAttribute("aria-labelledby"))
        {
            builder.AddAttribute(1, "aria-label", DefaultLabel);
        }

        builder.AddMultipleAttributes(2, AdditionalAttributes);
        builder.AddButtonAttributes(3, ButtonSemantics.IsNative(Element), "button", disabled);
        builder.AddDataDisabled(9, disabled);
        builder.AddAttribute(10, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, StepAsync));
        if (ChildContent is null)
        {
            builder.AddContent(11, DefaultText);
        }
        else
        {
            builder.AddContent(12, ChildContent);
        }

        builder.CloseElement();
    }

    private Task StepAsync() =>
        Context is not null && CanStep(Context) ? Context.SetPageAsync(Context.Page + Step) : Task.CompletedTask;
}
