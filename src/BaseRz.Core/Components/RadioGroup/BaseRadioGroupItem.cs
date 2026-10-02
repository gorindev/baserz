using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.RadioGroup;

/// <summary>
/// <c>role="radio"</c> button for <see cref="Value"/> with <c>aria-checked</c>. Only the checked radio (or the
/// first enabled one) is in the tab sequence.
/// </summary>
public class BaseRadioGroupItem : BaseRzComponent, IDisposable
{
    public const string Checked = "checked";
    public const string Unchecked = "unchecked";

    private RovingFocusItem? _focusItem;
    private ElementReference _element;

    [CascadingParameter] private BaseRadioGroupContext? Context { get; set; }

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
        var isChecked = Context?.IsChecked(Value) == true;
        var tabIndex = _focusItem is not null && Context is not null ? Context.Focus.TabIndexFor(_focusItem) : null;

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddControlAttributes(2, ButtonSemantics.IsNative(Element), "radio", IsDisabled, tabIndex);
        builder.AddAttribute(7, "aria-checked", isChecked ? "true" : "false");
        builder.AddAttribute(8, DataAttributes.DataState, isChecked ? Checked : Unchecked);
        builder.AddDataDisabled(9, IsDisabled);
        builder.AddAttribute(10, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, SelectAsync));
        builder.AddAttribute(11, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDownAsync));
        builder.AddElementReferenceCapture(12, element => _element = element);

        builder.OpenComponent<CascadingValue<BaseRadioGroupItemState>>(13);
        builder.AddComponentParameter(14, "Value", new BaseRadioGroupItemState(isChecked, IsDisabled));
        builder.AddComponentParameter(15, "ChildContent", ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        if (Context is null || _focusItem is null || IsDisabled)
        {
            return;
        }

        if (RovingFocusGroup.IsNavigationKey(args) && args.Key is not ("Home" or "End"))
        {
            var target = await Context.Focus.HandleKeyDownAsync(_focusItem, args);
            if (target?.Value is { } value)
            {
                await Context.SelectAsync(value);
            }
        }
        else if (!ButtonSemantics.IsNative(Element) && args.Key == " ")
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
