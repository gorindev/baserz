using BaseRz.Core;
using BaseRz.Core.Components;
using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Tests.Helpers.Probes;

public sealed class DataAttributeProbe : BaseRzComponent
{
    [Parameter] public bool Open { get; set; }

    [Parameter] public bool Disabled { get; set; }

    [Parameter] public Orientation Orientation { get; set; }

    [Parameter] public Side Side { get; set; }

    [Parameter] public Align Align { get; set; }

    public string GeneratedId => Id;

    protected override string IdPrefix => "probe";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "id", Id);
        builder.AddDataState(3, Open);
        builder.AddDataDisabled(4, Disabled);
        builder.AddDataOrientation(5, Orientation);
        builder.AddDataSide(6, Side);
        builder.AddDataAlign(7, Align);
        builder.AddContent(8, ChildContent);
        builder.CloseElement();
    }
}
