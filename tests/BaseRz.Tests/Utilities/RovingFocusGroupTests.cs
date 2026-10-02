using BaseRz.Core;
using BaseRz.Core.Utilities;
using BaseRz.Tests.Helpers;

namespace BaseRz.Tests.Utilities;

public class RovingFocusGroupTests
{
    private readonly List<string> _focused = [];
    private int _changes;

    private (RovingFocusGroup Group, RovingFocusItem[] Items) Create(params bool[] disabled)
    {
        var group = new RovingFocusGroup(() => _changes++);
        var items = disabled
            .Select((isDisabled, index) =>
            {
                var name = ((char)('a' + index)).ToString();
                var item = new RovingFocusItem(() => isDisabled, () => { _focused.Add(name); return Task.CompletedTask; }, name);
                group.Register(item);
                return item;
            })
            .ToArray();
        return (group, items);
    }

    [Fact]
    public void RovingFocusGroup_TabIndexOnlyActiveIsZero()
    {
        var (group, items) = Create(false, false, false);

        Assert.Equal(["0", "-1", "-1"], items.Select(group.TabIndexFor));

        group.SetActive(items[1]);
        Assert.Equal(["-1", "0", "-1"], items.Select(group.TabIndexFor));
        Assert.True(group.IsActive(items[1]));
        Assert.Equal(1, _changes);
    }

    [Fact]
    public void FallsBackToFirstEnabled_AndPrefersPreferredItem()
    {
        var (group, items) = Create(true, false, false);
        Assert.Equal(["-1", "0", "-1"], items.Select(group.TabIndexFor));

        group.Preferred = () => items[2];
        Assert.Equal(["-1", "-1", "0"], items.Select(group.TabIndexFor));

        group.Unregister(items[2]);
        Assert.Equal("0", group.TabIndexFor(items[1]));
    }

    [Fact]
    public async Task Horizontal_ArrowsMove_VerticalArrowsIgnored()
    {
        var (group, items) = Create(false, false, false);
        group.Orientation = Orientation.Horizontal;

        Assert.Same(items[1], await group.HandleKeyDownAsync(items[0], Keys.ArrowRight));
        Assert.Same(items[0], await group.HandleKeyDownAsync(items[1], Keys.ArrowLeft));
        Assert.Null(await group.HandleKeyDownAsync(items[0], Keys.ArrowDown));
        Assert.Null(await group.HandleKeyDownAsync(items[0], Keys.ArrowUp));
        Assert.Equal(["b", "a"], _focused);
        Assert.True(group.IsActive(items[0]));
    }

    [Fact]
    public async Task Vertical_ArrowsMove_HorizontalArrowsIgnored()
    {
        var (group, items) = Create(false, false, false);
        group.Orientation = Orientation.Vertical;

        Assert.Same(items[1], await group.HandleKeyDownAsync(items[0], Keys.ArrowDown));
        Assert.Same(items[0], await group.HandleKeyDownAsync(items[1], Keys.ArrowUp));
        Assert.Null(await group.HandleKeyDownAsync(items[0], Keys.ArrowRight));
        Assert.Null(await group.HandleKeyDownAsync(items[0], Keys.ArrowLeft));
    }

    [Fact]
    public async Task NoOrientation_AcceptsBothAxes()
    {
        var (group, items) = Create(false, false);

        Assert.Same(items[1], await group.HandleKeyDownAsync(items[0], Keys.ArrowDown));
        Assert.Same(items[0], await group.HandleKeyDownAsync(items[1], Keys.ArrowLeft));
    }

    [Fact]
    public async Task HomeEnd_JumpToEnabledEdges()
    {
        var (group, items) = Create(true, false, false, true);

        Assert.Same(items[2], await group.HandleKeyDownAsync(items[1], Keys.End));
        Assert.Same(items[1], await group.HandleKeyDownAsync(items[2], Keys.Home));
    }

    [Fact]
    public async Task SkipsDisabled_AndLoops()
    {
        var (group, items) = Create(false, true, false);
        group.Orientation = Orientation.Horizontal;

        Assert.Same(items[2], await group.HandleKeyDownAsync(items[0], Keys.ArrowRight));
        Assert.Same(items[0], await group.HandleKeyDownAsync(items[2], Keys.ArrowRight));
        Assert.Same(items[2], await group.HandleKeyDownAsync(items[0], Keys.ArrowLeft));
    }

    [Fact]
    public async Task LoopOff_StopsAtEdges()
    {
        var (group, items) = Create(false, false);
        group.Orientation = Orientation.Horizontal;
        group.Loop = false;

        Assert.Null(await group.HandleKeyDownAsync(items[1], Keys.ArrowRight));
        Assert.Null(await group.HandleKeyDownAsync(items[0], Keys.ArrowLeft));
        Assert.Empty(_focused);
    }

    [Fact]
    public async Task Rtl_SwapsLeftAndRight()
    {
        var (group, items) = Create(false, false, false);
        group.Orientation = Orientation.Horizontal;
        group.Direction = Direction.Rtl;
        group.Loop = false;

        Assert.Same(items[1], await group.HandleKeyDownAsync(items[0], Keys.ArrowLeft));
        Assert.Same(items[0], await group.HandleKeyDownAsync(items[1], Keys.ArrowRight));
    }

    [Fact]
    public async Task OtherKeys_AreIgnored_UnlessTypeaheadMatches()
    {
        var (group, items) = Create(false, false, false);

        Assert.Null(await group.HandleKeyDownAsync(items[0], Keys.Enter));
        Assert.Null(await group.HandleKeyDownAsync(items[0], Keys.Character('c')));

        group.Typeahead = key => items.FirstOrDefault(item => item.Value == key);
        Assert.Same(items[2], await group.HandleKeyDownAsync(items[0], Keys.Character('c')));
    }

    [Fact]
    public void IsNavigationKey_RecognizesArrowsHomeEnd()
    {
        Assert.True(RovingFocusGroup.IsNavigationKey(Keys.ArrowUp));
        Assert.True(RovingFocusGroup.IsNavigationKey(Keys.Home));
        Assert.False(RovingFocusGroup.IsNavigationKey(Keys.Enter));
    }
}
