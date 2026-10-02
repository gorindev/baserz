using AngleSharp.Dom;

using Bunit;

using Microsoft.AspNetCore.Components;

namespace BaseRz.Tests.Helpers;

/// <summary>Reads <c>ElementReference.FocusAsync</c> calls recorded by bUnit's JS interop.</summary>
public static class FocusAssert
{
    public const string FocusIdentifier = "Blazor._internal.domWrapper.focus";

    public static int FocusCount(BunitJSInterop jsInterop) =>
        jsInterop.Invocations.Count(invocation => invocation.Identifier == FocusIdentifier);
}

/// <summary>
/// Maps element references to readable names so tests can assert which element was focused. bUnit prints an
/// element's <c>blazor:elementReference</c> id only on its first render, so call <see cref="Observe"/> right after
/// the elements appear.
/// </summary>
public sealed class FocusTracker(BunitJSInterop jsInterop)
{
    private readonly Dictionary<string, string> _names = new(StringComparer.Ordinal);

    public FocusTracker Observe(IEnumerable<IElement> elements, Func<IElement, string> name)
    {
        foreach (var element in elements)
        {
            var reference = element.GetAttribute("blazor:elementReference");
            if (!string.IsNullOrEmpty(reference))
            {
                _names[reference] = name(element);
            }
        }

        return this;
    }

    public string? LastFocused
    {
        get
        {
            var last = jsInterop.Invocations.LastOrDefault(invocation => invocation.Identifier == FocusAssert.FocusIdentifier);
            if (last.Identifier != FocusAssert.FocusIdentifier)
            {
                return null;
            }

            var reference = Assert.IsType<ElementReference>(last.Arguments[0]);
            return _names.GetValueOrDefault(reference.Id, $"<unobserved {reference.Id}>");
        }
    }
}
