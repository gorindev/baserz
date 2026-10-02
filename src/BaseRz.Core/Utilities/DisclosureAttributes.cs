using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Utilities;

internal static class DisclosureAttributes
{
    /// <summary>Closed disclosure content gets <c>hidden</c> and <c>inert</c>. Uses sequences 0-1 relative to <paramref name="sequence"/>.</summary>
    public static void AddDisclosureHidden(this RenderTreeBuilder builder, int sequence, bool open)
    {
        if (open)
        {
            return;
        }

        builder.AddAttribute(sequence, "hidden", true);
        builder.AddAttribute(sequence + 1, "inert", true);
    }
}
