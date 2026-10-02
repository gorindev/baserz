using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Accordion;

/// <summary>One header + content pair, identified by <see cref="Value"/>.</summary>
public class BaseAccordionItem : BaseRzComponent
{
    private BaseAccordionItemContext? _item;

    [CascadingParameter] private BaseAccordionContext? Context { get; set; }

    [Parameter, EditorRequired] public string Value { get; set; } = string.Empty;

    [Parameter] public bool Disabled { get; set; }

    protected override string IdPrefix => "baserz-accordion";

    protected override void OnInitialized()
    {
        if (Context is not null)
        {
            _item = new BaseAccordionItemContext(Context, this, Id);
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddDataState(2, _item?.Open == true);
        builder.AddDataDisabled(3, _item?.Disabled ?? Disabled);
        if (Context is not null)
        {
            builder.AddDataOrientation(4, Context.Orientation);
        }

        builder.OpenComponent<CascadingValue<BaseAccordionItemContext?>>(5);
        builder.AddComponentParameter(6, "Value", _item);
        builder.AddComponentParameter(7, "ChildContent", ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }
}
