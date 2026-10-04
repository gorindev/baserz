using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BaseRz.Tests.Components.Form;

/// <summary>Writes FluentValidation results into the cascaded <see cref="EditContext"/> message store.</summary>
public sealed class TestFluentValidator<T> : ComponentBase, IDisposable where T : class
{
    private ValidationMessageStore? _messages;

    [CascadingParameter] public EditContext? EditContext { get; set; }

    [Parameter, EditorRequired] public IValidator<T> Validator { get; set; } = default!;

    protected override void OnInitialized()
    {
        ArgumentNullException.ThrowIfNull(EditContext);
        _messages = new ValidationMessageStore(EditContext);
        EditContext.OnValidationRequested += OnValidationRequested;
    }

    private void OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
    {
        _messages!.Clear();
        ValidationResult result = Validator.Validate((T)EditContext!.Model);
        foreach (var error in result.Errors)
        {
            _messages.Add(EditContext.Field(error.PropertyName), error.ErrorMessage);
        }

        EditContext.NotifyValidationStateChanged();
    }

    public void Dispose()
    {
        if (EditContext is not null)
        {
            EditContext.OnValidationRequested -= OnValidationRequested;
        }

        GC.SuppressFinalize(this);
    }
}
