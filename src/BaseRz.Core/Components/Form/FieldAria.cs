using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Form;

/// <summary>Copies field id and validation ARIA onto a control when the caller has not set them.</summary>
internal static class FieldAria
{
    public static void Apply(RenderTreeBuilder builder, int sequence, BaseFieldContext? field, Func<string, bool> hasAttribute, bool labelledBy)
    {
        if (field is null)
        {
            return;
        }

        if (!hasAttribute("id"))
        {
            builder.AddAttribute(sequence, "id", field.Id);
        }

        if (!hasAttribute("aria-invalid") && field.HasErrors)
        {
            builder.AddAttribute(sequence + 1, "aria-invalid", "true");
        }

        if (!hasAttribute("aria-describedby") && field.DescribedBy is not null)
        {
            builder.AddAttribute(sequence + 2, "aria-describedby", field.DescribedBy);
        }

        if (!hasAttribute("aria-required") && field.Required)
        {
            builder.AddAttribute(sequence + 3, "aria-required", "true");
        }

        if (labelledBy && !hasAttribute("aria-labelledby") && field.LabelId.Value is not null)
        {
            builder.AddAttribute(sequence + 4, "aria-labelledby", field.LabelId.Value);
        }
    }
}
