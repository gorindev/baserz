using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.Rating;

/// <summary><c>role="radio"</c>. Pointer and keyboard preview until activated.</summary>
public class BaseRatingItem : BaseRzComponent
{
    public const string Checked = "checked";
    public const string Preview = "preview";
    public const string Unchecked = "unchecked";

    [CascadingParameter] private BaseRatingContext? Context { get; set; }

    [Parameter, EditorRequired] public int Value { get; set; }

    [Parameter] public bool Disabled { get; set; }

    protected override string DefaultElement => "button";

    private bool IsDisabled => Disabled || Context?.Disabled == true;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var committed = Context?.Value == Value;
        var preview = Context?.Preview == Value && !committed;
        var state = committed ? Checked : preview ? Preview : Unchecked;

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "role", "radio");
        builder.AddAttribute(3, "type", "button");
        builder.AddAttribute(4, "aria-checked", committed ? "true" : "false");
        builder.AddAttribute(5, DataAttributes.DataState, state);
        if (IsDisabled)
        {
            builder.AddAttribute(6, "disabled", true);
        }

        builder.AddDataDisabled(7, IsDisabled);
        builder.AddAttribute(8, "onmouseenter", EventCallback.Factory.Create(this, () => Context?.SetPreview(Value)));
        builder.AddAttribute(9, "onclick", EventCallback.Factory.Create(this, () => Context?.CommitAsync(Value) ?? Task.CompletedTask));
        builder.AddAttribute(10, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, OnKeyDownAsync));
        builder.AddContent(11, ChildContent);
        builder.CloseElement();
    }

    private Task OnKeyDownAsync(KeyboardEventArgs args)
    {
        if (Context is null || IsDisabled)
        {
            return Task.CompletedTask;
        }

        if (args.Key is "Enter" or " ")
        {
            return Context.CommitAsync(Context.Preview ?? Value);
        }

        if (args.Key is "ArrowRight" or "ArrowUp")
        {
            Context.SetPreview((Context.Preview ?? Context.Value) + 1);
        }
        else if (args.Key is "ArrowLeft" or "ArrowDown")
        {
            Context.SetPreview(Math.Max(1, (Context.Preview ?? Context.Value) - 1));
        }

        return Task.CompletedTask;
    }
}
