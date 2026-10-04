using System.ComponentModel.DataAnnotations;
using System.Reflection;

using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components.Forms;

namespace BaseRz.Core.Components.Form;

/// <summary>Id, description/error registration, and validation flags for one <see cref="BaseField{TValue}"/>.</summary>
internal sealed class BaseFieldContext
{
    public BaseFieldContext(Action changed)
    {
        LabelId = new RegisteredId(changed);
        DescriptionId = new RegisteredId(changed);
        ErrorMessageId = new RegisteredId(changed);
        Changed = changed;
    }

    public Action Changed { get; }

    public string Id { get; set; } = "";

    public FieldIdentifier Field { get; set; }

    public EditContext? EditContext { get; set; }

    public bool IsValidating { get; set; }

    public bool Required { get; set; }

    public RegisteredId LabelId { get; }

    public RegisteredId DescriptionId { get; }

    public RegisteredId ErrorMessageId { get; }

    public bool IsModified => EditContext?.IsModified(Field) ?? false;

    public IReadOnlyList<string> Errors =>
        EditContext is null ? [] : EditContext.GetValidationMessages(Field).ToArray();

    public bool HasErrors => Errors.Count > 0;

    public bool IsValid => !HasErrors;

    public string? DescribedBy
    {
        get
        {
            var describedBy = string.Join(' ', new[] { DescriptionId.Value, ErrorMessageId.Value }
                .Where(static id => !string.IsNullOrWhiteSpace(id)));
            return describedBy.Length == 0 ? null : describedBy;
        }
    }

    public BaseFieldControlState ToControlState() => new()
    {
        Id = Id,
        DescribedBy = DescribedBy,
        AriaInvalid = HasErrors ? "true" : null,
        AriaRequired = Required ? "true" : null,
        IsModified = IsModified,
        IsValid = IsValid,
        IsValidating = IsValidating,
        HasErrors = HasErrors,
        Errors = Errors,
    };

    public void NotifyEditContext()
    {
        if (EditContext is not null)
        {
            EditContext.NotifyFieldChanged(Field);
        }
    }

    public static bool HasRequiredAttribute(FieldIdentifier field)
    {
        var type = field.Model.GetType();
        PropertyInfo? property = null;
        foreach (var name in field.FieldName.Split('.'))
        {
            property = type.GetProperty(name);
            if (property is null)
            {
                return false;
            }

            type = property.PropertyType;
        }

        return property?.GetCustomAttribute<RequiredAttribute>() is not null;
    }
}
