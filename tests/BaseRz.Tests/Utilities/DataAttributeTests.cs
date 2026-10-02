using BaseRz.Core;
using BaseRz.Core.Utilities;
using BaseRz.Tests.Helpers;
using BaseRz.Tests.Helpers.Probes;

using Bunit;

namespace BaseRz.Tests.Utilities;

public class DataAttributeTests : BaseRzTestContext
{
    [Fact]
    public void MapsOpenClosed_Disabled_Orientation_Side_Align()
    {
        Assert.Equal("open", DataAttributes.OpenClosed(true));
        Assert.Equal("closed", DataAttributes.OpenClosed(false));

        Assert.Equal("horizontal", DataAttributes.Of(Orientation.Horizontal));
        Assert.Equal("vertical", DataAttributes.Of(Orientation.Vertical));
        Assert.Equal("top", DataAttributes.Of(Side.Top));
        Assert.Equal("right", DataAttributes.Of(Side.Right));
        Assert.Equal("bottom", DataAttributes.Of(Side.Bottom));
        Assert.Equal("left", DataAttributes.Of(Side.Left));
        Assert.Equal("start", DataAttributes.Of(Align.Start));
        Assert.Equal("center", DataAttributes.Of(Align.Center));
        Assert.Equal("end", DataAttributes.Of(Align.End));
        Assert.Equal("ltr", DataAttributes.Of(Direction.Ltr));
        Assert.Equal("rtl", DataAttributes.Of(Direction.Rtl));

        var cut = Render<DataAttributeProbe>(parameters => parameters
            .Add(p => p.Open, true)
            .Add(p => p.Disabled, true)
            .Add(p => p.Orientation, Orientation.Vertical)
            .Add(p => p.Side, Side.Left)
            .Add(p => p.Align, Align.End));

        var element = cut.Find("div");
        Assert.Equal("open", element.GetAttribute(DataAttributes.DataState));
        Assert.True(element.HasAttribute(DataAttributes.DataDisabled));
        Assert.Equal("vertical", element.GetAttribute(DataAttributes.DataOrientation));
        Assert.Equal("left", element.GetAttribute(DataAttributes.DataSide));
        Assert.Equal("end", element.GetAttribute(DataAttributes.DataAlign));

        cut.Render(parameters => parameters
            .Add(p => p.Open, false)
            .Add(p => p.Disabled, false));

        element = cut.Find("div");
        Assert.Equal("closed", element.GetAttribute("data-state"));
        Assert.False(element.HasAttribute("data-disabled"));
    }

    [Fact]
    public void AttributeNames_AreDataPrefixed()
    {
        Assert.Equal("data-state", DataAttributes.DataState);
        Assert.Equal("data-disabled", DataAttributes.DataDisabled);
        Assert.Equal("data-orientation", DataAttributes.DataOrientation);
        Assert.Equal("data-side", DataAttributes.DataSide);
        Assert.Equal("data-align", DataAttributes.DataAlign);
    }
}
