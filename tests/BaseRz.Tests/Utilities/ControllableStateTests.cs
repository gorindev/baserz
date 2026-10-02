using BaseRz.Core.Utilities;
using BaseRz.Tests.Helpers;
using BaseRz.Tests.Helpers.Probes;

using Bunit;

using Microsoft.AspNetCore.Components;

namespace BaseRz.Tests.Utilities;

public class ControllableStateTests : BaseRzTestContext
{
    [Fact]
    public async Task Uncontrolled_UsesDefault_ThenIgnoresDefaultChanges()
    {
        var changes = new List<bool>();
        var state = new ControllableState<bool>();
        var changed = EventCallback.Factory.Create<bool>(this, changes.Add);

        state.Sync(isControlled: false, controlledValue: false, defaultValue: true, changed);
        Assert.False(state.IsControlled);
        Assert.True(state.Value);

        state.Sync(isControlled: false, controlledValue: false, defaultValue: false, changed);
        Assert.True(state.Value);

        Assert.True(await state.SetAsync(false));
        Assert.False(state.Value);
        Assert.Equal([false], changes);

        Assert.False(await state.SetAsync(false));
        Assert.Equal([false], changes);

        var cut = Render<StatefulProbe>(parameters => parameters.Add(p => p.DefaultOpen, true));
        Assert.Equal("open", cut.Find("button").GetAttribute("data-state"));
        Assert.False(cut.Instance.IsControlled);

        cut.Render(parameters => parameters.Add(p => p.DefaultOpen, false));
        Assert.Equal("open", cut.Find("button").GetAttribute("data-state"));

        cut.Find("button").Click();
        Assert.Equal("closed", cut.Find("button").GetAttribute("data-state"));
    }

    [Fact]
    public async Task Controlled_EmitsChanged_AndDoesNotMutateStaleParent()
    {
        var changes = new List<bool>();
        var state = new ControllableState<bool>();
        var changed = EventCallback.Factory.Create<bool>(this, changes.Add);

        state.Sync(isControlled: true, controlledValue: false, defaultValue: true, changed);
        Assert.True(state.IsControlled);
        Assert.False(state.Value);

        Assert.True(await state.SetAsync(true));
        Assert.Equal([true], changes);
        Assert.False(state.Value);

        state.Sync(isControlled: true, controlledValue: true, defaultValue: true, changed);
        Assert.True(state.Value);

        var emitted = new List<bool>();
        var cut = Render<StatefulProbe>(parameters => parameters
            .Add(p => p.Open, false)
            .Add(p => p.OpenChanged, emitted.Add));

        Assert.True(cut.Instance.IsControlled);
        cut.Find("button").Click();
        Assert.Equal([true], emitted);
        Assert.Equal("closed", cut.Find("button").GetAttribute("data-state"));

        cut.Render(parameters => parameters
            .Add(p => p.Open, true)
            .Add(p => p.OpenChanged, emitted.Add));
        Assert.Equal("open", cut.Find("button").GetAttribute("data-state"));
    }

    [Fact]
    public void Controlled_Bind_ParentWinsOnRerender()
    {
        var parentValue = false;
        var cut = Render<StatefulProbe>(parameters => parameters
            .Add(p => p.Open, parentValue)
            .Add(p => p.OpenChanged, value => parentValue = value));

        cut.Find("button").KeyDown(Keys.Enter);
        Assert.True(parentValue);

        cut.Render(parameters => parameters
            .Add(p => p.Open, parentValue)
            .Add(p => p.OpenChanged, value => parentValue = value));
        Assert.Equal("open", cut.Find("button").GetAttribute("data-state"));
    }

    [Fact]
    public void Disabled_IgnoresInput()
    {
        var cut = Render<StatefulProbe>(parameters => parameters.Add(p => p.Disabled, true));
        var button = cut.Find("button");

        button.Click();
        cut.Find("button").KeyDown(Keys.Space);
        cut.Find("button").KeyDown(Keys.Enter);

        Assert.Equal("closed", cut.Find("button").GetAttribute("data-state"));
        Assert.True(cut.Find("button").HasAttribute("data-disabled"));
    }
}
