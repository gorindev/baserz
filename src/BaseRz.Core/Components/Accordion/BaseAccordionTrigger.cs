using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.Accordion;

/// <summary>
/// Button that toggles its item: <c>aria-expanded</c>, <c>aria-controls</c>, and <c>aria-disabled</c> when the item
/// is disabled or is an open item that cannot be collapsed.
/// </summary>
public class BaseAccordionTrigger : BaseRzComponent, IDisposable
{
    private RovingFocusItem? _focusItem;
    private ElementReference _element;

    [CascadingParameter] private BaseAccordionItemContext? Item { get; set; }

    protected override string DefaultElement => "button";

    protected override void OnInitialized()
    {
        if (Item is not null)
        {
            _focusItem = new RovingFocusItem(() => Item?.Disabled ?? true, () => _element.FocusAsync().AsTask(), Item.Value);
            Item.Root.Focus.Register(_focusItem);
        }
    }

    protected override void OnParametersSet() => Item?.TriggerId.Use(AdditionalAttributes);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var native = ButtonSemantics.IsNative(Element);
        var open = Item?.Open == true;
        var disabled = Item?.Disabled == true;
        var locked = Item?.Locked == true;

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddButtonAttributes(2, native, "button", disabled);
        builder.AddAttribute(8, "id", Item?.TriggerId.Value);
        builder.AddAttribute(9, "aria-expanded", open ? "true" : "false");
        builder.AddAttribute(10, "aria-controls", Item?.ContentId.Value);
        if ((disabled && native) || (locked && !disabled))
        {
            builder.AddAttribute(11, "aria-disabled", "true");
        }

        builder.AddDataState(12, open);
        builder.AddDataDisabled(13, disabled);
        if (Item is not null)
        {
            builder.AddDataOrientation(14, Item.Root.Orientation);
        }

        builder.AddAttribute(15, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ToggleAsync));
        builder.AddAttribute(16, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDownAsync));
        builder.AddElementReferenceCapture(17, element => _element = element);
        builder.AddContent(18, ChildContent);
        builder.CloseElement();
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        if (Item is null || _focusItem is null)
        {
            return;
        }

        if (RovingFocusGroup.IsNavigationKey(args))
        {
            await Item.Root.Focus.HandleKeyDownAsync(_focusItem, args);
        }
        else if (!ButtonSemantics.IsNative(Element) && ButtonSemantics.IsActivationKey(args))
        {
            await ToggleAsync();
        }
    }

    private Task ToggleAsync() =>
        Item is null || Item.Disabled || Item.Locked ? Task.CompletedTask : Item.Root.ToggleAsync(Item.Value);

    public void Dispose()
    {
        if (_focusItem is not null)
        {
            Item?.Root.Focus.Unregister(_focusItem);
        }

        GC.SuppressFinalize(this);
    }
}
