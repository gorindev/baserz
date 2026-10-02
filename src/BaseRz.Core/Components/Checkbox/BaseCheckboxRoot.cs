using BaseRz.Core.Components.CheckboxGroup;
using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.Checkbox;

/// <summary>
/// Custom <c>role="checkbox"</c> (not a native input) with checked, unchecked, and indeterminate
/// (<c>aria-checked="mixed"</c>) states. Activating an indeterminate checkbox clears indeterminate and checks it.
/// Inside a <c>BaseCheckboxGroupRoot</c>, <see cref="Value"/> membership in the group's values drives the state.
/// </summary>
public class BaseCheckboxRoot : BaseRzComponent
{
    public const string CheckedState = "checked";
    public const string UncheckedState = "unchecked";
    public const string IndeterminateState = "indeterminate";

    private readonly ControllableState<bool> _checked = new();
    private readonly ControllableState<bool> _indeterminate = new();

    [CascadingParameter] private BaseCheckboxGroupContext? Group { get; set; }

    [Parameter] public bool Checked { get; set; }

    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }

    [Parameter] public bool DefaultChecked { get; set; }

    [Parameter] public bool Indeterminate { get; set; }

    [Parameter] public EventCallback<bool> IndeterminateChanged { get; set; }

    [Parameter] public bool DefaultIndeterminate { get; set; }

    [Parameter] public bool Disabled { get; set; }

    [Parameter] public bool Required { get; set; }

    /// <summary>Form field name; when set a hidden input posts <see cref="Value"/> while checked. Defaults to the group's name.</summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>Posted value, and the item's identity inside a checkbox group.</summary>
    [Parameter] public string Value { get; set; } = "on";

    protected override string DefaultElement => "button";

    private bool InGroup => Group is not null && IsParameterSet(nameof(Value));

    private bool IsChecked => InGroup ? Group!.Contains(Value) : _checked.Value;

    private bool IsDisabled => Disabled || Group?.Disabled == true;

    protected override void OnParametersSet()
    {
        _checked.Sync(IsParameterSet(nameof(Checked)), Checked, DefaultChecked, CheckedChanged);
        _indeterminate.Sync(IsParameterSet(nameof(Indeterminate)), Indeterminate, DefaultIndeterminate, IndeterminateChanged);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var state = _indeterminate.Value ? IndeterminateState : IsChecked ? CheckedState : UncheckedState;

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddControlAttributes(2, ButtonSemantics.IsNative(Element), "checkbox", IsDisabled);
        builder.AddAttribute(7, "aria-checked", _indeterminate.Value ? "mixed" : IsChecked ? "true" : "false");
        if (Required)
        {
            builder.AddAttribute(8, "aria-required", "true");
        }

        builder.AddAttribute(9, DataAttributes.DataState, state);
        builder.AddDataDisabled(10, IsDisabled);
        builder.AddAttribute(11, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ToggleAsync));
        builder.AddAttribute(12, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDownAsync));

        builder.OpenComponent<CascadingValue<BaseCheckboxState>>(13);
        builder.AddComponentParameter(14, "Value", new BaseCheckboxState(state, IsDisabled));
        builder.AddComponentParameter(15, "ChildContent", ChildContent);
        builder.CloseComponent();
        builder.CloseElement();

        if (IsChecked && !_indeterminate.Value)
        {
            builder.AddHiddenInput(16, Name ?? Group?.Name, Value);
        }
    }

    private Task HandleKeyDownAsync(KeyboardEventArgs args) =>
        !ButtonSemantics.IsNative(Element) && args.Key == " " ? ToggleAsync() : Task.CompletedTask;

    private async Task ToggleAsync()
    {
        if (IsDisabled)
        {
            return;
        }

        var next = true;
        if (_indeterminate.Value)
        {
            await _indeterminate.SetAsync(false);
        }
        else
        {
            next = !IsChecked;
        }

        if (InGroup)
        {
            await Group!.SetAsync(Value, next);
        }
        else
        {
            await _checked.SetAsync(next);
        }

        StateHasChanged();
    }
}

/// <summary>State read by <see cref="BaseCheckboxIndicator"/>.</summary>
internal sealed record BaseCheckboxState(string State, bool Disabled);
