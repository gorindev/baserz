namespace BaseRz.Core.Utilities;

/// <summary>Helpers for components that must emit structural inline styles alongside the caller's <c>style</c>.</summary>
internal static class InlineStyle
{
    public static Dictionary<string, object>? AttributesWithoutStyle(Dictionary<string, object>? additionalAttributes)
    {
        if (additionalAttributes is null)
        {
            return null;
        }

        var copy = new Dictionary<string, object>(additionalAttributes);
        copy.Remove("style");
        return copy;
    }

    /// <summary>Component style first, then the caller's <c>style</c> so caller declarations win.</summary>
    public static string Merge(Dictionary<string, object>? additionalAttributes, string ownStyle)
    {
        if (additionalAttributes is not null
            && additionalAttributes.TryGetValue("style", out var value)
            && value is string callerStyle
            && !string.IsNullOrWhiteSpace(callerStyle))
        {
            return $"{ownStyle.TrimEnd(';', ' ')}; {callerStyle}";
        }

        return ownStyle;
    }
}
