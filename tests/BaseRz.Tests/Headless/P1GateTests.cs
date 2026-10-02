using System.Text.RegularExpressions;

using BaseRz.Core.Components;
using BaseRz.Tests.Helpers;

using Microsoft.AspNetCore.Components;

namespace BaseRz.Tests.Headless;

/// <summary>Phase gate for P1 in docs/roadmap.md: parts coverage, headless source scan, samples, ARIA notes.</summary>
public partial class P1GateTests
{
    public static readonly string[] P1Roots =
    [
        "BaseVisuallyHidden", "BaseSeparator", "BaseLabel", "BaseAspectRatio", "BaseButton", "BaseToggle",
        "BaseAlert", "BaseAvatar", "BaseImage", "BaseProgress", "BaseTable", "BaseBreadcrumb", "BasePagination",
    ];

    public static TheoryData<string> P1RootData => new(P1Roots);

    private static readonly string[] ForbiddenTokens =
        ["ButtonColor", "ButtonSize", "ButtonVariant", "ToastColor", "TableStyles", "TableVariant"];

    [Theory]
    [MemberData(nameof(P1RootData))]
    public void EveryP1PartInComponentsMd_HasPublicType(string root)
    {
        var parts = ComponentsMdParts(root);
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

    [Fact]
    public void CoreSource_EmitsNoOwnClass_AndNoDesignTokens()
    {
        var sourceRoot = Path.Combine(RepoRoot.Find(), "src", "BaseRz.Core");
        var files = Directory
            .EnumerateFiles(sourceRoot, "*.*", SearchOption.AllDirectories)
            .Where(static path => path.EndsWith(".cs", StringComparison.Ordinal) || path.EndsWith(".razor", StringComparison.Ordinal))
            .Where(static path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                && !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToList();
        Assert.NotEmpty(files);

        foreach (var file in files)
        {
            var source = File.ReadAllText(file);
            Assert.False(ClassAttributeEmit().IsMatch(source), $"{file} emits its own class attribute");
            foreach (var token in ForbiddenTokens)
            {
                Assert.False(source.Contains(token, StringComparison.Ordinal), $"{file} references design token {token}");
            }

            foreach (Match literal in StringLiteral().Matches(source))
            {
                HeadlessAssert.AssertNoUtilityClasses(literal.Groups[1].Value);
            }
        }
    }

    [Theory]
    [MemberData(nameof(P1RootData))]
    public void EveryP1Component_HasSamplePage(string root)
    {
        var name = root["Base".Length..];
        var page = Path.Combine(RepoRoot.Find(), "samples", "BaseRz.Samples.Wasm", "Pages", "Components", $"{name}Page.razor");

        Assert.True(File.Exists(page), $"Missing sample page {page}");
        Assert.Contains($"@page \"/components/{Kebab(name)}\"", File.ReadAllText(page));
        Assert.Contains($"<{root}Root", File.ReadAllText(page));
    }

    [Theory]
    [MemberData(nameof(P1RootData))]
    public void EveryP1Component_HasAriaNotes(string root)
    {
        var notes = File.ReadAllText(Path.Combine(RepoRoot.Find(), "docs", "aria-notes.md"));

        Assert.Contains($"## {root}", notes);
    }

    public static string Kebab(string name) => KebabBoundary().Replace(name, "-$1").ToLowerInvariant();

    internal static List<string> ComponentsMdParts(string root)
    {
        var lines = File.ReadAllLines(Path.Combine(RepoRoot.Find(), "docs", "components.md"));
        var parts = new List<string>();
        var inSection = false;
        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                inSection = line[3..].Trim() == root;
                continue;
            }

            if (inSection && line.StartsWith("* ", StringComparison.Ordinal))
            {
                parts.Add(line[2..].Trim());
            }
        }

        return parts;
    }

    [GeneratedRegex("""AddAttribute\(\s*\d+\s*,\s*"class"\s*,""")]
    private static partial Regex ClassAttributeEmit();

    [GeneratedRegex("\"((?:[^\"\\\\]|\\\\.)*)\"")]
    private static partial Regex StringLiteral();

    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex KebabBoundary();
}
