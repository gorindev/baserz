namespace BaseRz.Core.Components.Form;

/// <summary>How <see cref="BaseFieldErrorMessage"/> renders the field's validation messages.</summary>
public enum BaseFieldErrorMode
{
    /// <summary>The first message, as a single alert.</summary>
    Single,

    /// <summary>Every message, as a list.</summary>
    All,
}
