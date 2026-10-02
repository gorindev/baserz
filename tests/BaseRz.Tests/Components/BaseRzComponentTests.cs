using BaseRz.Tests.Helpers;
using BaseRz.Tests.Helpers.Probes;

using Bunit;

namespace BaseRz.Tests.Components;

public class BaseRzComponentTests : BaseRzTestContext
{
    [Fact]
    public void As_OverridesDefaultElement()
    {
        var byDefault = Render<StatefulProbe>();
        Assert.Equal("BUTTON", byDefault.Find("[data-state]").TagName);

        var asLink = Render<StatefulProbe>(parameters => parameters.Add(p => p.As, "a"));
        Assert.Equal("A", asLink.Find("[data-state]").TagName);

        var probe = Render<DataAttributeProbe>(parameters => parameters.Add(p => p.As, "section"));
        Assert.Equal("SECTION", probe.Find("[data-state]").TagName);
    }

    [Fact]
    public void CallerClass_Only()
    {
        var withClass = Render<StatefulProbe>(parameters => parameters.AddUnmatched("class", "my-trigger"));
        Assert.Equal("my-trigger", withClass.Find("button").GetAttribute("class"));

        var withoutClass = Render<DataAttributeProbe>();
        var element = withoutClass.Find("div");
        Assert.False(element.HasAttribute("class"));
        var markup = withoutClass.Markup;
        Assert.DoesNotContain(HeadlessAssert.ForbiddenClassFragments, fragment => markup.Contains($"class=\"{fragment}", StringComparison.Ordinal));
    }

    [Fact]
    public void UnmatchedAttributes_PassThrough()
    {
        var cut = Render<DataAttributeProbe>(parameters => parameters
            .AddUnmatched("id", "host")
            .AddUnmatched("data-test", "probe")
            .AddUnmatched("aria-label", "Probe"));

        var element = cut.Find("div");
        Assert.Equal("host", element.Id);
        Assert.Equal("probe", element.GetAttribute("data-test"));
        Assert.Equal("Probe", element.GetAttribute("aria-label"));
    }

    [Fact]
    public void IsParameterSet_TracksSuppliedParameters()
    {
        var cut = this.RenderWithCascading<StatefulProbe, ProbeCascade>(
            new ProbeCascade("root"),
            parameters => parameters.Add(p => p.DefaultOpen, true));

        Assert.Equal("root", cut.Find("button").GetAttribute("data-cascade"));
        Assert.True(cut.Instance.WasSet(nameof(StatefulProbe.DefaultOpen)));
        Assert.False(cut.Instance.WasSet(nameof(StatefulProbe.Open)));
        Assert.False(cut.Instance.WasSet(nameof(StatefulProbe.Cascade)));
        Assert.False(cut.Instance.IsControlled);

        cut.Find("button").KeyDown(Keys.Space);
        Assert.Equal("closed", cut.Find("button").GetAttribute("data-state"));

        cut.Find("button").KeyDown(Keys.Escape);
        Assert.Equal("closed", cut.Find("button").GetAttribute("data-state"));
    }
}
