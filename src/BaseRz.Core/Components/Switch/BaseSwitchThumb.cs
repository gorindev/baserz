using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Switch;

/// <summary>The moving part of the switch; mirrors the root's <c>data-state</c>.</summary>
public class BaseSwitchThumb : BaseRzComponent
{
    [CascadingParameter] private BaseSwitchState? Switch { get; set; }

    protected override string DefaultElement => "span";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, DataAttributes.DataState, Switch?.Checked == true ? BaseSwitchRoot.CheckedState : BaseSwitchRoot.UncheckedState);
        builder.AddDataDisabled(3, Switch?.Disabled == true);
        builder.AddContent(4, ChildContent);
        builder.CloseElement();
    }
}
