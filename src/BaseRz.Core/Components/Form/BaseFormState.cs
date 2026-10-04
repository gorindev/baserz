using Microsoft.AspNetCore.Components.Forms;

namespace BaseRz.Core.Components.Form;

/// <summary>Form flags cascaded by <see cref="BaseFormRoot"/> and passed to its child content.</summary>
public sealed class BaseFormState
{
    public EditContext EditContext { get; internal set; } = default!;

    public bool IsModified { get; internal set; }

    public bool IsSubmitting { get; internal set; }

    public bool IsValid { get; internal set; } = true;

    public bool IsValidating { get; internal set; }

    public bool HasErrors { get; internal set; }
}
