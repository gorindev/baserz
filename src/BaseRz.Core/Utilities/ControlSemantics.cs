using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Utilities;

/// <summary>
/// Attributes for custom controls built on a button (<c>checkbox</c>, <c>radio</c>, <c>switch</c>, <c>tab</c>): the
/// role is always emitted; a native <c>&lt;button&gt;</c> also gets <c>type="button"</c> and <c>disabled</c>, any
/// other element gets a tab stop and <c>aria-disabled</c>.
/// </summary>
internal static class ControlSemantics
{
    /// <summary>
    /// Uses sequences 0-4 relative to <paramref name="sequence"/>. A non-null <paramref name="rovingTabIndex"/> is
    /// emitted on native buttons too, for parts in a roving-focus group.
    /// </summary>
    public static void AddControlAttributes(
        this RenderTreeBuilder builder,
        int sequence,
        bool native,
        string role,
        bool disabled,
        string? rovingTabIndex = null)
    {
        builder.AddAttribute(sequence, "role", role);
        if (native)
        {
            builder.AddAttribute(sequence + 1, "type", "button");
            builder.AddAttribute(sequence + 2, "disabled", disabled);
            if (rovingTabIndex is not null)
            {
                builder.AddAttribute(sequence + 3, "tabindex", rovingTabIndex);
            }

            return;
        }

        builder.AddAttribute(sequence + 3, "tabindex", disabled ? "-1" : rovingTabIndex ?? "0");
        if (disabled)
        {
            builder.AddAttribute(sequence + 4, "aria-disabled", "true");
        }
    }

    /// <summary>Hidden form field so a custom control still posts with a native form. Emitted only when <paramref name="name"/> is set.</summary>
    public static void AddHiddenInput(this RenderTreeBuilder builder, int sequence, string? name, string? value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        builder.OpenElement(sequence, "input");
        builder.AddAttribute(sequence + 1, "type", "hidden");
        builder.AddAttribute(sequence + 2, "name", name);
        builder.AddAttribute(sequence + 3, "value", value ?? string.Empty);
        builder.CloseElement();
    }
}
