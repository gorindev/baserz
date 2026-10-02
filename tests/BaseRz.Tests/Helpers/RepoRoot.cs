namespace BaseRz.Tests.Helpers;

public static class RepoRoot
{
    public static string Find()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "BaseRz.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException($"BaseRz.slnx not found above {AppContext.BaseDirectory}");
    }
}
