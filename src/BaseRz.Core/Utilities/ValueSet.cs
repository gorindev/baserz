namespace BaseRz.Core.Utilities;

/// <summary>Ordinal string-set helpers for multi-value state (<c>Values</c> on Accordion, ToggleGroup, CheckboxGroup, TreeView).</summary>
internal static class ValueSet
{
    public static readonly IEqualityComparer<IReadOnlyCollection<string>> Comparer = new SetComparer();

    public static bool Contains(IReadOnlyCollection<string>? values, string? value) =>
        value is not null && values is not null && values.Contains(value, StringComparer.Ordinal);

    /// <summary>Copy of <paramref name="values"/> with <paramref name="value"/> added or removed, keeping order.</summary>
    public static IReadOnlyCollection<string> With(IReadOnlyCollection<string>? values, string value, bool include)
    {
        var next = (values ?? []).Where(existing => !string.Equals(existing, value, StringComparison.Ordinal)).ToList();
        if (include)
        {
            next.Add(value);
        }

        return next;
    }

    public static IReadOnlyCollection<string> Toggle(IReadOnlyCollection<string>? values, string value) =>
        With(values, value, !Contains(values, value));

    private sealed class SetComparer : IEqualityComparer<IReadOnlyCollection<string>>
    {
        public bool Equals(IReadOnlyCollection<string>? x, IReadOnlyCollection<string>? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return x.ToHashSet(StringComparer.Ordinal).SetEquals(y);
        }

        public int GetHashCode(IReadOnlyCollection<string> obj) => obj.Count;
    }
}
