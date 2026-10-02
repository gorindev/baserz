using System.Text.RegularExpressions;

using AngleSharp.Dom;

namespace BaseRz.Tests.Helpers;

public static partial class HeadlessAssert
{
    public static readonly string[] ForbiddenClassFragments =
        ["flex", "bg-", "text-", "rounded-", "p-", "gap-", "border-", "shadow-", "hover:", "data-["];

    /// <summary>Every <c>class</c> value in <paramref name="markup"/> must be one of <paramref name="allowed"/>.</summary>
    public static void AssertOnlyCallerClasses(string markup, params string[] allowed)
    {
        foreach (Match match in ClassAttribute().Matches(markup))
        {
            var value = match.Groups[1].Value;
            Assert.Contains(value, allowed);
            AssertNoUtilityClasses(value);
        }
    }

    /// <summary>Asserts the attributes passed as <c>id</c>, <c>data-test="probe"</c>, <c>aria-label="Probe"</c> reached <paramref name="element"/>.</summary>
    public static void AssertPassThrough(IElement element, string id)
    {
        Assert.Equal(id, element.Id);
        Assert.Equal("probe", element.GetAttribute("data-test"));
        Assert.Equal("Probe", element.GetAttribute("aria-label"));
    }

    public static void AssertNoUtilityClasses(string classValue)
    {
        foreach (var token in classValue.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            Assert.DoesNotContain(ForbiddenClassFragments, fragment => token.StartsWith(fragment, StringComparison.Ordinal));
        }
    }

    [GeneratedRegex("class=\"([^\"]*)\"")]
    private static partial Regex ClassAttribute();
}
