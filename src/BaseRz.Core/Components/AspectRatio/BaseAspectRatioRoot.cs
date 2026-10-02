using System.Globalization;
using System.Text.RegularExpressions;

using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.AspectRatio;

/// <summary>Constrains its box to <see cref="Ratio"/> (<c>"w:h"</c>) with an inline <c>aspect-ratio</c>.</summary>
public partial class BaseAspectRatioRoot : BaseRzComponent
{
    /// <summary>Width-to-height ratio such as <c>"16:9"</c>. Invalid values fall back to <c>1:1</c>.</summary>
    [Parameter] public string? Ratio { get; set; } = "1:1";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var (x, y) = ParseRatio(Ratio);
        var width = x.ToString(CultureInfo.InvariantCulture);
        var height = y.ToString(CultureInfo.InvariantCulture);

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, InlineStyle.AttributesWithoutStyle(AdditionalAttributes));
        builder.AddAttribute(2, "style", InlineStyle.Merge(AdditionalAttributes, $"aspect-ratio: {width} / {height}"));
        builder.AddAttribute(3, "data-ratio", $"{width}:{height}");
        builder.AddContent(4, ChildContent);
        builder.CloseElement();
    }

    public static (double X, double Y) ParseRatio(string? ratio)
    {
        if (string.IsNullOrWhiteSpace(ratio))
        {
            return (1, 1);
        }

        var match = RatioPattern().Match(ratio);
        if (!match.Success
            || !double.TryParse(match.Groups[1].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var x)
            || !double.TryParse(match.Groups[2].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var y)
            || x <= 0
            || y <= 0)
        {
            return (1, 1);
        }

        return (x, y);
    }

    [GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)\s*:\s*(\d+(?:\.\d+)?)\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex RatioPattern();
}
