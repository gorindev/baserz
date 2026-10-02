namespace BaseRz.Core.Utilities;

public static class Css
{
    public static string Join(params string?[] parts) =>
        string.Join(" ", parts.Where(static part => !string.IsNullOrWhiteSpace(part)));

    public static string? CallerClass(Dictionary<string, object>? additionalAttributes)
    {
        if (additionalAttributes is not null
            && additionalAttributes.TryGetValue("class", out var value)
            && value is string className
            && !string.IsNullOrWhiteSpace(className))
        {
            return className;
        }

        return null;
    }

    public static Dictionary<string, object>? AttributesWithoutClass(Dictionary<string, object>? additionalAttributes)
    {
        if (additionalAttributes is null)
        {
            return null;
        }

        var copy = new Dictionary<string, object>(additionalAttributes);
        copy.Remove("class");
        return copy;
    }
}
