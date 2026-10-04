using System.Text.Json;

using BaseRz.Core.Components;
using BaseRz.Tests.Helpers;

using Microsoft.AspNetCore.Components;

namespace BaseRz.Tests.Headless;

/// <summary>Phase gate for P3 in docs/roadmap.md: parts, samples, ARIA notes, and slider.js.</summary>
public class P3GateTests
{
    public static readonly string[] P3Roots =
    [
        "BaseForm", "BaseInput", "BaseNumberField", "BaseSlider", "BaseRating", "BaseTagInput", "BaseSegmentedInput",
    ];

    public static TheoryData<string> P3RootData => new(P3Roots);

    [Theory]
    [MemberData(nameof(P3RootData))]
    public void EveryP3PartInComponentsMd_HasPublicType(string root)
    {
        var parts = P1GateTests.ComponentsMdParts(root);
        Assert.NotEmpty(parts);

        var assembly = typeof(BaseRzComponentCore).Assembly;
        foreach (var part in parts)
        {
            var typeName = part == "BaseField"
                ? "BaseRz.Core.Components.Form.BaseField`1"
                : $"BaseRz.Core.Components.{root["Base".Length..]}.{part}";
            var type = assembly.GetType(typeName);
            Assert.True(type is not null, $"Missing part {part} for {root} ({typeName})");
            Assert.True(type.IsPublic && !type.IsAbstract, $"{part} must be a public concrete type");
            Assert.True(typeof(IComponent).IsAssignableFrom(type), $"{part} must be a component");
        }
    }

    [Theory]
    [MemberData(nameof(P3RootData))]
    public void EveryP3Component_HasSamplePage(string root)
    {
        var name = root["Base".Length..];
        var page = Path.Combine(RepoRoot.Find(), "samples", "BaseRz.Samples.Wasm", "Pages", "Components", $"{name}Page.razor");

        Assert.True(File.Exists(page), $"Missing sample page {page}");
        var text = File.ReadAllText(page);
        Assert.Contains($"@page \"/components/{P1GateTests.Kebab(name)}\"", text);
        Assert.Contains($"<{root}Root", text);
    }

    [Theory]
    [MemberData(nameof(P3RootData))]
    public void EveryP3Component_HasAriaNotes(string root)
    {
        var notes = File.ReadAllText(Path.Combine(RepoRoot.Find(), "docs", "aria-notes.md"));

        Assert.Contains("## P3 — Form and fields", notes);
        Assert.Contains($"## {root}", notes);
    }

    [Fact]
    public void SliderJs_IsPackagedAsStaticWebAsset()
    {
        var jsProject = Path.Combine(RepoRoot.Find(), "src", "BaseRz.JS");
        var source = Path.Combine(jsProject, "wwwroot", "slider.js");
        Assert.True(File.Exists(source), $"Missing {source}");

        var manifest = Directory
            .EnumerateFiles(Path.Combine(jsProject, "obj"), "staticwebassets.build.json", SearchOption.AllDirectories)
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .FirstOrDefault();
        Assert.NotNull(manifest);

        using var document = JsonDocument.Parse(File.ReadAllText(manifest));
        var asset = document.RootElement
            .GetProperty("Assets")
            .EnumerateArray()
            .FirstOrDefault(static asset =>
                asset.GetProperty("SourceType").GetString() == "Discovered"
                && Path.GetFullPath(asset.GetProperty("Identity").GetString()!)
                    .EndsWith(Path.Combine("wwwroot", "slider.js"), StringComparison.OrdinalIgnoreCase));

        Assert.NotEqual(JsonValueKind.Undefined, asset.ValueKind);
        Assert.Equal("_content/BaseRz.JS", asset.GetProperty("BasePath").GetString());
    }
}
