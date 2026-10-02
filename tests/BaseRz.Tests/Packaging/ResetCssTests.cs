using System.Text.Json;

using BaseRz.Tests.Helpers;

namespace BaseRz.Tests.Packaging;

public class ResetCssTests
{
    private const string ResetRelativePath = "css/baserz-reset.css";

    [Fact]
    public void BaserzReset_IsPackagedAsStaticWebAsset()
    {
        var coreProject = Path.Combine(RepoRoot.Find(), "src", "BaseRz.Core");
        var source = Path.Combine(coreProject, "wwwroot", "css", "baserz-reset.css");
        Assert.True(File.Exists(source), $"Missing {source}");

        var manifest = Directory
            .EnumerateFiles(Path.Combine(coreProject, "obj"), "staticwebassets.build.json", SearchOption.AllDirectories)
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
                    .EndsWith(Path.Combine("wwwroot", "css", "baserz-reset.css"), StringComparison.OrdinalIgnoreCase));

        Assert.NotEqual(JsonValueKind.Undefined, asset.ValueKind);
        Assert.Equal("_content/BaseRz.Core", asset.GetProperty("BasePath").GetString());
        Assert.Equal(ResetRelativePath, StripFingerprint(asset.GetProperty("RelativePath").GetString()!));
    }

    private static string StripFingerprint(string relativePath)
    {
        var start = relativePath.IndexOf("#[", StringComparison.Ordinal);
        if (start < 0)
        {
            return relativePath;
        }

        var end = relativePath.IndexOf(']', start);
        var close = end + 1 < relativePath.Length && relativePath[end + 1] == '?' ? end + 2 : end + 1;
        return string.Concat(relativePath.AsSpan(0, start), relativePath.AsSpan(close));
    }
}
