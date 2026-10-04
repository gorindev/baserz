using BaseRz.Core.Components.Form;
using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.Switch;

/// <summary>
/// On/off control with <c>role="switch"</c> and <c>aria-checked</c>. Controlled via <c>@bind-Checked</c> or
/// uncontrolled via <see cref="DefaultChecked"/>. Set <see cref="Name"/> to post <see cref="Value"/> while on.
/// </summary>
public class BaseSwitchRoot : BaseRzComponent
{
    public const string CheckedState = "checked";
    public const string UncheckedState = "unchecked";

    private readonly ControllableState<bool> _checked = new();

    [CascadingParameter] private BaseFieldContext? Field { get; set; }

    [Parameter] public bool Checked { get; set; }

    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }

    [Parameter] public bool DefaultChecked { get; set; }

    [Parameter] public bool Disabled { get; set; }

    [Parameter] public bool Required { get; set; }

    [Parameter] public string? Name { get; set; }

    [Parameter] public string Value { get; set; } = "on";

    protected override string DefaultElement => "button";

    protected override void OnParametersSet()
    {
        _checked.Sync(IsParameterSet(nameof(Checked)), Checked, DefaultChecked, CheckedChanged);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var on = _checked.Value;

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddControlAttributes(2, ButtonSemantics.IsNative(Element), "switch", Disabled);
        builder.AddAttribute(7, "aria-checked", on ? "true" : "false");
        if (Required)
        {
            builder.AddAttribute(8, "aria-required", "true");
        }

        builder.AddAttribute(9, DataAttributes.DataState, on ? CheckedState : UncheckedState);
        builder.AddDataDisabled(10, Disabled);
        builder.AddAttribute(11, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ToggleAsync));
        builder.AddAttribute(12, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDownAsync));
        FieldAria.Apply(builder, 13, Field, HasAttribute, labelledBy: true);

        builder.OpenComponent<CascadingValue<BaseSwitchState>>(20);
        builder.AddComponentParameter(21, "Value", new BaseSwitchState(on, Disabled));
        builder.AddComponentParameter(22, "ChildContent", ChildContent);
        builder.CloseComponent();
        builder.CloseElement();

        if (on)
        {
            builder.AddHiddenInput(16, Name, Value);
        }
    }

    private Task HandleKeyDownAsync(KeyboardEventArgs args) =>
        !ButtonSemantics.IsNative(Element) && ButtonSemantics.IsActivationKey(args) ? ToggleAsync() : Task.CompletedTask;

    private async Task ToggleAsync()
    {
        if (Disabled)
        {
            return;
        }

        await _checked.SetAsync(!_checked.Value);
        Field?.NotifyEditContext();
    }
}

/// <summary>State read by <see cref="BaseSwitchThumb"/>.</summary>
internal sealed record BaseSwitchState(bool Checked, bool Disabled);
