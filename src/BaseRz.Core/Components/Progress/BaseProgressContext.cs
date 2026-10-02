using System.Globalization;

using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Progress;

/// <summary>Normalized progress values shared by the root and indicator.</summary>
public sealed class BaseProgressContext
{
    public const string Indeterminate = "indeterminate";
    public const string Loading = "loading";
    public const string Complete = "complete";

    /// <summary>Clamped value, or <see langword="null"/> when indeterminate.</summary>
    public double? Value { get; internal set; }

    public double Min { get; internal set; }

    public double Max { get; internal set; } = 100;

    public string State => Value is not { } value ? Indeterminate : value >= Max ? Complete : Loading;

    /// <summary>Completion from 0 to 100, or <see langword="null"/> when indeterminate.</summary>
    public double? Percentage => Value is { } value ? (value - Min) / (Max - Min) * 100 : null;

    internal void AddDataAttributes(RenderTreeBuilder builder, int sequence)
    {
        builder.AddAttribute(sequence, DataAttributes.DataState, State);
        if (Value is { } value)
        {
            builder.AddAttribute(sequence + 1, "data-value", Format(value));
        }

        builder.AddAttribute(sequence + 2, "data-max", Format(Max));
    }

    internal static string Format(double value) => value.ToString(CultureInfo.InvariantCulture);
}
