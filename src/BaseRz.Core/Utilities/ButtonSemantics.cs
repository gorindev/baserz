using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Utilities;

/// <summary>
/// APG button rules shared by button-like parts. A native <c>&lt;button&gt;</c> gets <c>type</c> and
/// <c>disabled</c>; any other element gets <c>role="button"</c>, a tab stop, and Enter/Space activation.
/// </summary>
internal static class ButtonSemantics
{
    public static bool IsNative(string element) => string.Equals(element, "button", StringComparison.OrdinalIgnoreCase);

    public static bool IsActivationKey(KeyboardEventArgs args) => args.Key is "Enter" or " " || args.Code is "Space";

    /// <summary>Adds <c>type</c>/<c>disabled</c> or <c>role</c>/<c>tabindex</c>/<c>aria-disabled</c>. Uses sequences 0-5 relative to <paramref name="sequence"/>.</summary>
    public static void AddButtonAttributes(
        this RenderTreeBuilder builder,
        int sequence,
        bool native,
        string type,
        bool disabled,
        int tabIndex = 0)
    {
        if (native)
        {
            builder.AddAttribute(sequence, "type", type);
            if (disabled)
            {
                builder.AddAttribute(sequence + 1, "disabled", true);
            }

            return;
        }

        builder.AddAttribute(sequence + 2, "role", "button");
        builder.AddAttribute(sequence + 3, "tabindex", disabled ? "-1" : tabIndex.ToString(System.Globalization.CultureInfo.InvariantCulture));
        if (disabled)
        {
            builder.AddAttribute(sequence + 4, "aria-disabled", "true");
        }
    }
}
