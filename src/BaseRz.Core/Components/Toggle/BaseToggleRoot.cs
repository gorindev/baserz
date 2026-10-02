using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.Toggle;

/// <summary>Two-state button with <c>aria-pressed</c>. Controlled via <c>@bind-Pressed</c> or uncontrolled via <see cref="DefaultPressed"/>.</summary>
public class BaseToggleRoot : BaseRzComponent
{
    public const string On = "on";
    public const string Off = "off";

    private readonly ControllableState<bool> _pressed = new();

    [Parameter] public bool Pressed { get; set; }

    [Parameter] public EventCallback<bool> PressedChanged { get; set; }

    [Parameter] public bool DefaultPressed { get; set; }

    [Parameter] public bool Disabled { get; set; }

    protected override string DefaultElement => "button";

    protected override void OnParametersSet()
    {
        _pressed.Sync(IsParameterSet(nameof(Pressed)), Pressed, DefaultPressed, PressedChanged);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var native = ButtonSemantics.IsNative(Element);

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddButtonAttributes(2, native, "button", Disabled);
        builder.AddAttribute(8, "aria-pressed", _pressed.Value ? "true" : "false");
        builder.AddAttribute(9, DataAttributes.DataState, _pressed.Value ? On : Off);
        builder.AddDataDisabled(10, Disabled);
        builder.AddAttribute(11, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ToggleAsync));
        builder.AddAttribute(12, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDownAsync));
        builder.AddContent(13, ChildContent);
        builder.CloseElement();
    }

    private Task HandleKeyDownAsync(KeyboardEventArgs args) =>
        !ButtonSemantics.IsNative(Element) && ButtonSemantics.IsActivationKey(args) ? ToggleAsync() : Task.CompletedTask;

    private async Task ToggleAsync()
    {
        if (Disabled)
        {
            return;
        }

        await _pressed.SetAsync(!_pressed.Value);
    }
}
