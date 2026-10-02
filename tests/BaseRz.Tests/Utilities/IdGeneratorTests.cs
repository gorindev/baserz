using BaseRz.Core.Utilities;
using BaseRz.Tests.Helpers;
using BaseRz.Tests.Helpers.Probes;

using Bunit;

namespace BaseRz.Tests.Utilities;

public class IdGeneratorTests : BaseRzTestContext
{
    [Fact]
    public void IsStablePerInstance_AndUniqueAcrossInstances()
    {
        var first = Render<DataAttributeProbe>();
        var firstId = first.Find("div").Id;

        first.Render(parameters => parameters.Add(p => p.Open, true));
        Assert.Equal(firstId, first.Find("div").Id);
        Assert.Equal(firstId, first.Instance.GeneratedId);

        var second = Render<DataAttributeProbe>();
        Assert.NotEqual(firstId, second.Find("div").Id);

        Assert.StartsWith("probe-", firstId);
        Assert.StartsWith("baserz-", IdGenerator.Next());
        Assert.NotEqual(IdGenerator.Next("x"), IdGenerator.Next("x"));
    }

    [Fact]
    public void CallerId_WinsOverGeneratedId()
    {
        var cut = Render<DataAttributeProbe>(parameters => parameters.AddUnmatched("id", "mine"));

        Assert.Equal("mine", cut.Find("div").Id);
        Assert.Equal("mine", cut.Instance.GeneratedId);
    }
}
