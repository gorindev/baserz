using System.Diagnostics;
using System.IO.Compression;
using System.Xml.Linq;

using BaseRz.Tests.Helpers;

namespace BaseRz.Tests.Packaging;

public class PackagingTests
{
    [Fact]
    public async Task CoreAndJs_HaveAuthorsLicenseReadme()
    {
        var root = RepoRoot.Find();
        var output = Path.Combine(Path.GetTempPath(), "baserz-pack-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(output);

        try
        {
            await PackAsync(root, Path.Combine(root, "src", "BaseRz.JS", "BaseRz.JS.csproj"), output);
            await PackAsync(root, Path.Combine(root, "src", "BaseRz.Core", "BaseRz.Core.csproj"), output);

            var props = XDocument.Load(Path.Combine(root, "src", "Directory.Build.props"));
            var authors = props.Descendants().Single(element => element.Name.LocalName == "Authors").Value;
            var copyright = props.Descendants().Single(element => element.Name.LocalName == "Copyright").Value;

            AssertPackage(output, "BaseRz.JS", dependsOnJs: false, authors, copyright);
            AssertPackage(output, "BaseRz.Core", dependsOnJs: true, authors, copyright);
        }
        finally
        {
            Directory.Delete(output, recursive: true);
        }
    }

    private static async Task PackAsync(string root, string project, string output)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"pack \"{project}\" -c Release -o \"{output}\" --nologo",
            WorkingDirectory = root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        Assert.True(process.Start());
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        var log = await stdout + await stderr;
        Assert.True(process.ExitCode == 0, log);
    }

    private static void AssertPackage(string output, string packageId, bool dependsOnJs, string authors, string copyright)
    {
        var nupkg = Directory.EnumerateFiles(output, packageId + ".*.nupkg").Single();
        using var archive = ZipFile.OpenRead(nupkg);
        var nuspec = archive.Entries.Single(entry => entry.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase));
        using var stream = nuspec.Open();
        var metadata = XDocument.Load(stream).Root!
            .Elements()
            .Single(element => element.Name.LocalName == "metadata");

        Assert.Equal(authors, Value(metadata, "authors"));
        Assert.Equal(copyright, Value(metadata, "copyright"));

        var license = metadata.Elements().Single(element => element.Name.LocalName == "license");
        Assert.Equal("expression", license.Attribute("type")?.Value);
        Assert.Equal("MIT", license.Value);
        Assert.Equal("README.md", Value(metadata, "readme"));

        var readme = archive.GetEntry("README.md");
        Assert.NotNull(readme);
        using var readmeStream = readme.Open();
        using var reader = new StreamReader(readmeStream);
        Assert.Contains("dotnet add package BaseRz.Core", reader.ReadToEnd(), StringComparison.Ordinal);

        var dependencyIds = metadata
            .Descendants()
            .Where(element => element.Name.LocalName == "dependency")
            .Select(element => element.Attribute("id")?.Value)
            .ToList();

        if (dependsOnJs)
        {
            Assert.Contains("BaseRz.JS", dependencyIds);
        }
        else
        {
            Assert.DoesNotContain("BaseRz.JS", dependencyIds);
        }
    }

    private static string Value(XElement metadata, string name)
    {
        return metadata.Elements().Single(element => element.Name.LocalName == name).Value;
    }
}
