using System.Globalization;

using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.Button;

/// <summary>
/// APG button. Renders a native <c>&lt;button&gt;</c> by default; with <c>As</c> set to another element it adds
/// <c>role="button"</c>, a tab stop, and Enter/Space activation. <see cref="Loading"/> keeps focus but blocks
/// activation and sets <c>aria-busy</c>.
/// </summary>
public class BaseButtonRoot : BaseRzComponent
{
    /// <summary>Native button type (<c>button</c>, <c>submit</c>, <c>reset</c>). Ignored for non-button elements.</summary>
    [Parameter] public string Type { get; set; } = "button";

    [Parameter] public bool Disabled { get; set; }

    [Parameter] public bool Loading { get; set; }

    /// <summary>Tab index for non-button elements (always <c>-1</c> when disabled). Applied to a native button only when set.</summary>
    [Parameter] public int TabIndex { get; set; }

    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    protected override string DefaultElement => "button";

    private bool Inert => Disabled || Loading;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var native = ButtonSemantics.IsNative(Element);

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddButtonAttributes(2, native, Type, Disabled, TabIndex);
        if (native && IsParameterSet(nameof(TabIndex)))
        {
            builder.AddAttribute(8, "tabindex", TabIndex.ToString(CultureInfo.InvariantCulture));
        }

        if (Loading)
        {
            builder.AddAttribute(9, "aria-disabled", "true");
            builder.AddAttribute(10, "aria-busy", "true");
            builder.AddAttribute(11, "data-loading", string.Empty);
        }

        builder.AddDataDisabled(12, Disabled);
        builder.AddAttribute(13, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, HandleClickAsync));
        builder.AddAttribute(14, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDownAsync));
        builder.AddContent(15, ChildContent);
        builder.CloseElement();
    }

    private Task HandleClickAsync(MouseEventArgs args) => Inert ? Task.CompletedTask : OnClick.InvokeAsync(args);

    private Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        if (Inert || ButtonSemantics.IsNative(Element) || !ButtonSemantics.IsActivationKey(args))
        {
            return Task.CompletedTask;
        }

        return OnClick.InvokeAsync(new MouseEventArgs { Type = "click", Detail = 0 });
    }
}
