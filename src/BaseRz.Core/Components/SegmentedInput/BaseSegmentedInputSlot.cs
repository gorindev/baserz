using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.SegmentedInput;

/// <summary>One character. Typing advances, paste fills from this slot, Backspace clears and moves back.</summary>
public class BaseSegmentedInputSlot : BaseRzComponent
{
    [CascadingParameter] private BaseSegmentedInputContext? Context { get; set; }

    [Parameter] public int Index { get; set; }

    protected override string DefaultElement => "input";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var value = Context is not null && Index < Context.Value.Length ? Context.Value[Index].ToString() : "";
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "type", "text");
        builder.AddAttribute(3, "inputmode", "text");
        builder.AddAttribute(4, "value", value);
        builder.AddAttribute(5, "maxlength", Context is null ? 1 : Math.Max(1, Context.Length - Index));
        builder.SetUpdatesAttributeName("value");
        if (Context?.Disabled == true)
        {
            builder.AddAttribute(6, "disabled", true);
        }

        builder.AddAttribute(7, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, OnInputAsync));
        builder.AddAttribute(8, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, OnKeyDownAsync));
        builder.AddElementReferenceCapture(9, element =>
        {
            if (Context is not null && Index >= 0 && Index < Context.Slots.Length)
            {
                Context.Slots[Index] = element;
            }
        });
        builder.CloseElement();
    }

    private Task OnInputAsync(ChangeEventArgs args)
    {
        var text = args.Value?.ToString() ?? "";
        return Context?.SetAsync(Index, text) ?? Task.CompletedTask;
    }

    private Task OnKeyDownAsync(KeyboardEventArgs args)
    {
        if (args.Key == "Backspace")
        {
            return Context?.ClearAsync(Index) ?? Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}
