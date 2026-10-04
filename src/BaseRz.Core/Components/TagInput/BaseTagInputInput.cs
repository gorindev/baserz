using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.TagInput;

/// <summary>Text box that adds a tag on Enter and removes the last tag on Backspace when empty.</summary>
public class BaseTagInputInput : BaseRzComponent
{
    private string _text = "";

    [CascadingParameter] private BaseTagInputContext? Context { get; set; }

    protected override string DefaultElement => "input";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "type", "text");
        builder.AddAttribute(3, "value", _text);
        builder.SetUpdatesAttributeName("value");
        if (Context?.Disabled == true)
        {
            builder.AddAttribute(4, "disabled", true);
        }

        builder.AddAttribute(5, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, args => _text = args.Value?.ToString() ?? ""));
        builder.AddAttribute(6, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, OnKeyDownAsync));
        builder.CloseElement();
    }

    private async Task OnKeyDownAsync(KeyboardEventArgs args)
    {
        if (Context is null || Context.Disabled)
        {
            return;
        }

        if (args.Key == "Enter")
        {
            await Context.AddAsync(_text);
            _text = "";
            StateHasChanged();
        }
        else if (args.Key == "Backspace" && _text.Length == 0)
        {
            await Context.RemoveLastAsync();
        }
    }
}
