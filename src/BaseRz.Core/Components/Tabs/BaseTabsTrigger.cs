using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.Tabs;

/// <summary>
/// <c>role="tab"</c> for <see cref="Value"/>. Only the selected tab (or the first enabled one) is in the tab
/// sequence; arrows, Home and End move focus and select.
/// </summary>
public class BaseTabsTrigger : BaseRzComponent, IDisposable
{
    public const string Active = "active";
    public const string Inactive = "inactive";

    private RovingFocusItem? _focusItem;
    private ElementReference _element;

    [CascadingParameter] private BaseTabsContext? Context { get; set; }

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

        Context?.UseTriggerId(Value, AdditionalAttributes);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var selected = Context?.IsSelected(Value) == true;
        var tabIndex = _focusItem is not null && Context is not null ? Context.Focus.TabIndexFor(_focusItem) : null;

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddControlAttributes(2, ButtonSemantics.IsNative(Element), "tab", IsDisabled, tabIndex);
        builder.AddAttribute(7, "id", Context?.TriggerId(Value));
        builder.AddAttribute(8, "aria-controls", Context?.ContentId(Value));
        builder.AddAttribute(9, "aria-selected", selected ? "true" : "false");
        builder.AddAttribute(10, DataAttributes.DataState, selected ? Active : Inactive);
        builder.AddDataDisabled(11, IsDisabled);
        if (Context is not null)
        {
            builder.AddDataOrientation(12, Context.Orientation);
        }

        builder.AddAttribute(13, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, SelectAsync));
        builder.AddAttribute(14, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDownAsync));
        builder.AddElementReferenceCapture(15, element => _element = element);
        builder.AddContent(16, ChildContent);
        builder.CloseElement();
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        if (Context is null || _focusItem is null || IsDisabled)
        {
            return;
        }

        if (RovingFocusGroup.IsNavigationKey(args))
        {
            var target = await Context.Focus.HandleKeyDownAsync(_focusItem, args);
            if (target?.Value is { } value)
            {
                await Context.SelectAsync(value);
            }
        }
        else if (!ButtonSemantics.IsNative(Element) && ButtonSemantics.IsActivationKey(args))
        {
            await SelectAsync();
        }
    }

    private Task SelectAsync() => Context is null || IsDisabled ? Task.CompletedTask : Context.SelectAsync(Value);

    public void Dispose()
    {
        if (_focusItem is not null)
        {
            Context?.Focus.Unregister(_focusItem);
        }

        GC.SuppressFinalize(this);
    }
}
