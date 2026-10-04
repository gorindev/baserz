using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.Slider;

/// <summary><c>role="slider"</c>. Arrows step; Home and End jump to min and max.</summary>
public class BaseSliderThumb : BaseRzComponent
{
    [CascadingParameter] private BaseSliderContext? Context { get; set; }

    protected override string DefaultElement => "div";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var disabled = Context?.Disabled == true;
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "role", "slider");
        builder.AddAttribute(3, "tabindex", disabled ? "-1" : "0");
        if (Context is not null)
        {
            builder.AddAttribute(4, "aria-valuemin", BaseSliderRoot.Format(Context.Min));
            builder.AddAttribute(5, "aria-valuemax", BaseSliderRoot.Format(Context.Max));
            builder.AddAttribute(6, "aria-valuenow", BaseSliderRoot.Format(Context.Value));
            builder.AddAttribute(7, "aria-orientation", DataAttributes.Of(Context.Orientation));
        }

        if (disabled)
        {
            builder.AddAttribute(8, "aria-disabled", "true");
        }

        builder.AddDataDisabled(9, disabled);
        builder.AddAttribute(10, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, OnKeyDownAsync));
        if (Context?.ThumbStyle is not null)
        {
            builder.AddAttribute(11, "style", Context.ThumbStyle);
        }

        builder.AddContent(12, ChildContent);
        builder.CloseElement();
    }

    private Task OnKeyDownAsync(KeyboardEventArgs args)
    {
        if (Context is null || Context.Disabled)
        {
            return Task.CompletedTask;
        }

        var horizontal = Context.Orientation == Orientation.Horizontal;
        return args.Key switch
        {
            "ArrowRight" when horizontal => Context.StepAsync(1),
            "ArrowLeft" when horizontal => Context.StepAsync(-1),
            "ArrowUp" => Context.StepAsync(1),
            "ArrowDown" => Context.StepAsync(-1),
            "Home" => Context.SetAsync(Context.Min),
            "End" => Context.SetAsync(Context.Max),
            _ => Task.CompletedTask,
        };
    }
}
