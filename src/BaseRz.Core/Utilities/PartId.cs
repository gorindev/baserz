namespace BaseRz.Core.Utilities;

/// <summary>
/// Id of a part that other parts reference (<c>aria-controls</c>, <c>aria-labelledby</c>). Starts as an id the
/// root generates; the part replaces it with the caller's <c>id</c> attribute when one is passed.
/// </summary>
internal sealed class PartId(string generated, Action changed)
{
    private string? _callerId;

    public string Value => _callerId ?? generated;

    public void Use(Dictionary<string, object>? additionalAttributes)
    {
        var callerId = CallerId(additionalAttributes);
        if (string.Equals(_callerId, callerId, StringComparison.Ordinal))
        {
            return;
        }

        _callerId = callerId;
        changed();
    }

    public static string? CallerId(Dictionary<string, object>? additionalAttributes) =>
        additionalAttributes is not null
        && additionalAttributes.TryGetValue("id", out var value)
        && value is string id
        && !string.IsNullOrWhiteSpace(id)
            ? id
            : null;
}
