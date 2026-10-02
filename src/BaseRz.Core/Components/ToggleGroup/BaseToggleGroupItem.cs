using BaseRz.Core.Components.Toggle;
using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.ToggleGroup;

/// <summary>Toggle button for <see cref="Value"/> with <c>aria-pressed</c> and <c>data-state="on|off"</c>.</summary>
public class BaseToggleGroupItem : BaseRzComponent, IDisposable
{
    private RovingFocusItem? _focusItem;
    private ElementReference _element;

    [CascadingParameter] private BaseToggleGroupContext? Context { get; set; }

    [Parameter, EditorRequired] public string Value { get; set; } = string.Empty;

    [Parameter] public bool Disabled { get; set; }

    protected override string DefaultElement => "button";

    private bool IsDisabled => Disabled || Context?.Disabled == true;

    protected override void OnInitialized()
    {
        if (Context is not null)
        {
            _focusItem = new RovingFocusItem(() => IsDisabled, () => _element.FocusAsync().AsTask(), Value);
            Context.Focus.Register(_focusItem);
        }
    }

    protected override void OnParametersSet()
    {
        if (_focusItem is not null)
        {
            _focusItem.Value = Value;
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var native = ButtonSemantics.IsNative(Element);
        var pressed = Context?.IsPressed(Value) == true;
        var roving = Context?.RovingFocus == true && _focusItem is not null;
        var tabIndex = roving ? Context!.Focus.TabIndexFor(_focusItem!) : "0";

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddButtonAttributes(2, native, "button", IsDisabled, tabIndex == "0" ? 0 : -1);
        if (native && roving)
        {
            builder.AddAttribute(8, "tabindex", tabIndex);
        }

        builder.AddAttribute(9, "aria-pressed", pressed ? "true" : "false");
        builder.AddAttribute(10, DataAttributes.DataState, pressed ? BaseToggleRoot.On : BaseToggleRoot.Off);
        builder.AddDataDisabled(11, IsDisabled);
        if (Context is not null)
        {
            builder.AddDataOrientation(12, Context.Orientation);
        }

        builder.AddAttribute(13, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ToggleAsync));
        builder.AddAttribute(14, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDownAsync));
        builder.AddElementReferenceCapture(15, element => _element = element);
        builder.AddContent(16, ChildContent);
        builder.CloseElement();
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        if (Context is null || IsDisabled)
        {
            return;
        }

        if (Context.RovingFocus && _focusItem is not null && RovingFocusGroup.IsNavigationKey(args))
        {
            await Context.Focus.HandleKeyDownAsync(_focusItem, args);
        }
        else if (!ButtonSemantics.IsNative(Element) && ButtonSemantics.IsActivationKey(args))
        {
            await ToggleAsync();
        }
    }

    private async Task ToggleAsync()
    {
        if (Context is null || IsDisabled)
        {
            return;
        }

        if (_focusItem is not null)
        {
            Context.Focus.SetActive(_focusItem);
        }

        await Context.ToggleAsync(Value);
    }

    public void Dispose()
    {
        if (_focusItem is not null)
        {
            Context?.Focus.Unregister(_focusItem);
        }

        GC.SuppressFinalize(this);
    }
}
