using System.Text.RegularExpressions;

using BaseRz.Core.Components;
using BaseRz.Core.Components.Checkbox;
using BaseRz.Core.Components.CheckboxGroup;
using BaseRz.Core.Components.RadioGroup;
using BaseRz.Core.Components.Switch;
using BaseRz.Tests.Helpers;

using Bunit;

using Microsoft.AspNetCore.Components;

namespace BaseRz.Tests.Headless;

/// <summary>Phase gate for P2 in docs/roadmap.md: parts coverage, samples, ARIA notes, no native checkbox/radio controls.</summary>
public partial class P2GateTests : BaseRzTestContext
{
    public static readonly string[] P2Roots =
    [
        "BaseCollapsible", "BaseAccordion", "BaseTabs", "BaseToggleGroup", "BaseTreeView",
        "BaseRadioGroup", "BaseCheckbox", "BaseCheckboxGroup", "BaseSwitch",
    ];

    public static TheoryData<string> P2RootData => new(P2Roots);

    [Theory]
    [MemberData(nameof(P2RootData))]
    public void EveryP2PartInComponentsMd_HasPublicType(string root)
    {
        var parts = P1GateTests.ComponentsMdParts(root);
        Assert.NotEmpty(parts);

        var assembly = typeof(BaseRzComponentCore).Assembly;
        foreach (var part in parts)
        {
            var type = assembly.GetType($"BaseRz.Core.Components.{root["Base".Length..]}.{part}");
            Assert.True(type is not null, $"Missing part {part} for {root}");
            Assert.True(type.IsPublic && !type.IsAbstract, $"{part} must be a public concrete type");
            Assert.True(typeof(IComponent).IsAssignableFrom(type), $"{part} must be a component");
        }
    }

    [Theory]
    [MemberData(nameof(P2RootData))]
    public void EveryP2Component_HasSamplePage(string root)
    {
        var name = root["Base".Length..];
        var page = Path.Combine(RepoRoot.Find(), "samples", "BaseRz.Samples.Wasm", "Pages", "Components", $"{name}Page.razor");

        Assert.True(File.Exists(page), $"Missing sample page {page}");
        var text = File.ReadAllText(page);
        Assert.Contains($"@page \"/components/{P1GateTests.Kebab(name)}\"", text);
        Assert.Contains($"<{root}Root", text);
    }

    [Theory]
    [MemberData(nameof(P2RootData))]
    public void EveryP2Component_HasAriaNotes(string root)
    {
        var notes = File.ReadAllText(Path.Combine(RepoRoot.Find(), "docs", "aria-notes.md"));

        Assert.Contains("## P2 — Disclosure and roving focus", notes);
        Assert.Contains($"## {root}", notes);
    }

    [Fact]
    public void NoNativeCheckboxOrRadioInput_AsVisibleControl()
    {
        var checkbox = Render<BaseCheckboxRoot>(parameters => parameters
            .Add(p => p.DefaultChecked, true)
            .Add(p => p.Name, "accept")
            .Add(p => p.Value, "yes")
            .AddUnmatched("aria-label", "Accept")
            .AddChildContent<BaseCheckboxIndicator>());
        AssertNoVisibleCheckboxOrRadio(checkbox);

        var radios = Render<BaseRadioGroupRoot>(parameters => parameters
            .Add(p => p.DefaultValue, "a")
            .Add(p => p.Name, "choice")
            .AddUnmatched("aria-label", "Choice")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<BaseRadioGroupItem>(0);
                builder.AddComponentParameter(1, nameof(BaseRadioGroupItem.Value), "a");
                builder.AddAttribute(2, "aria-label", "A");
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(inner =>
                {
                    inner.OpenComponent<BaseRadioGroupIndicator>(0);
                    inner.CloseComponent();
                }));
                builder.CloseComponent();

                builder.OpenComponent<BaseRadioGroupItem>(4);
                builder.AddComponentParameter(5, nameof(BaseRadioGroupItem.Value), "b");
                builder.AddAttribute(6, "aria-label", "B");
                builder.AddAttribute(7, "ChildContent", (RenderFragment)(inner =>
                {
                    inner.OpenComponent<BaseRadioGroupIndicator>(0);
                    inner.CloseComponent();
                }));
                builder.CloseComponent();
            }));
        AssertNoVisibleCheckboxOrRadio(radios);

        var switchCut = Render<BaseSwitchRoot>(parameters => parameters
            .Add(p => p.DefaultChecked, true)
            .Add(p => p.Name, "on")
            .AddUnmatched("aria-label", "On")
            .AddChildContent<BaseSwitchThumb>());
        AssertNoVisibleCheckboxOrRadio(switchCut);

        var group = Render<BaseCheckboxGroupRoot>(parameters => parameters
            .Add(p => p.DefaultValues, new[] { "a" })
            .Add(p => p.Name, "opts")
            .AddUnmatched("aria-label", "Opts")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<BaseCheckboxGroupLabel>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(inner => inner.AddContent(0, "Opts")));
                builder.CloseComponent();

                builder.OpenComponent<BaseCheckboxRoot>(2);
                builder.AddComponentParameter(3, nameof(BaseCheckboxRoot.Value), "a");
                builder.AddAttribute(4, "aria-label", "A");
                builder.AddAttribute(5, "ChildContent", (RenderFragment)(inner =>
                {
                    inner.OpenComponent<BaseCheckboxIndicator>(0);
                    inner.CloseComponent();
                }));
                builder.CloseComponent();

                builder.OpenComponent<BaseCheckboxRoot>(6);
                builder.AddComponentParameter(7, nameof(BaseCheckboxRoot.Value), "b");
                builder.AddAttribute(8, "aria-label", "B");
                builder.AddAttribute(9, "ChildContent", (RenderFragment)(inner =>
                {
                    inner.OpenComponent<BaseCheckboxIndicator>(0);
                    inner.CloseComponent();
                }));
                builder.CloseComponent();
            }));
        AssertNoVisibleCheckboxOrRadio(group);

        var sourceRoot = Path.Combine(RepoRoot.Find(), "src", "BaseRz.Core");
        foreach (var file in Directory.EnumerateFiles(sourceRoot, "*.cs", SearchOption.AllDirectories)
                     .Where(static path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                         && !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)))
        {
            var source = File.ReadAllText(file);
            Assert.False(NativeCheckboxOrRadioType().IsMatch(source), $"{file} emits a visible native checkbox/radio type");
        }
    }

    private static void AssertNoVisibleCheckboxOrRadio<T>(IRenderedComponent<T> cut) where T : IComponent
    {
        Assert.Empty(cut.FindAll("input[type=checkbox]"));
        Assert.Empty(cut.FindAll("input[type=radio]"));
        foreach (var input in cut.FindAll("input"))
        {
            Assert.Equal("hidden", input.GetAttribute("type"));
        }
    }

    [GeneratedRegex("""AddAttribute\(\s*\d+\s*,\s*"type"\s*,\s*"(checkbox|radio)"\s*\)""")]
    private static partial Regex NativeCheckboxOrRadioType();
}
