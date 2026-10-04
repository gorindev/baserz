namespace BaseRz.Core.Components.Form;

/// <summary>Field flags and ARIA values passed to <see cref="BaseFieldControl"/> child content.</summary>
public sealed class BaseFieldControlState
{
    public required string Id { get; init; }

    public string? DescribedBy { get; init; }

    /// <summary><c>true</c> when the field has errors; otherwise null so the attribute is omitted.</summary>
    public string? AriaInvalid { get; init; }

    /// <summary><c>true</c> when the field is required; otherwise null.</summary>
    public string? AriaRequired { get; init; }

    public bool IsModified { get; init; }

    public bool IsValid { get; init; }

    public bool IsValidating { get; init; }

    public bool HasErrors { get; init; }

    public IReadOnlyList<string> Errors { get; init; } = [];
}
