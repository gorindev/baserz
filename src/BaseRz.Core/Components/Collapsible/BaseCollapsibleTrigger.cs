using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.Collapsible;

/// <summary>Button that toggles the collapsible, with <c>aria-expanded</c> and <c>aria-controls</c>.</summary>
public class BaseCollapsibleTrigger : BaseRzComponent
{
    [CascadingParameter] private BaseCollapsibleContext? Context { get; set; }

    protected override string DefaultElement => "button";

    protected override void OnParametersSet() => Context?.TriggerId.Use(AdditionalAttributes);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var open = Context?.Open == true;
        var disabled = Context?.Disabled == true;

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddButtonAttributes(2, ButtonSemantics.IsNative(Element), "button", disabled);
        builder.AddAttribute(8, "id", Context?.TriggerId.Value);
        builder.AddAttribute(9, "aria-expanded", open ? "true" : "false");
        builder.AddAttribute(10, "aria-controls", Context?.ContentId.Value);
        builder.AddDataState(11, open);
        builder.AddDataDisabled(12, disabled);
        builder.AddAttribute(13, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ToggleAsync));
        builder.AddAttribute(14, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDownAsync));
        builder.AddContent(15, ChildContent);
        builder.CloseElement();
    }

    private Task HandleKeyDownAsync(KeyboardEventArgs args) =>
        !ButtonSemantics.IsNative(Element) && ButtonSemantics.IsActivationKey(args) ? ToggleAsync() : Task.CompletedTask;

    private Task ToggleAsync() => Context is null || Context.Disabled ? Task.CompletedTask : Context.ToggleAsync();
}
