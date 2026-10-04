using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Form;

/// <summary>
/// Headless <see cref="EditForm"/>. Owns the <see cref="Microsoft.AspNetCore.Components.Forms.EditContext"/> and
/// exposes <see cref="IsModified"/>, <see cref="IsSubmitting"/>, <see cref="IsValid"/>, <see cref="IsValidating"/>,
/// and <see cref="HasErrors"/> to child content and on the <c>&lt;form&gt;</c>.
/// </summary>
public class BaseFormRoot : BaseRzTemplatedComponent<BaseFormState>, IDisposable
{
    private readonly BaseFormState _state = new();
    private EditContext? _active;
    private EditContext? _subscribed;
    private CancellationTokenSource? _validation;

    [Parameter] public object? Model { get; set; }

    [Parameter] public EditContext? EditContext { get; set; }

    [Parameter] public EventCallback<EditContext> OnSubmit { get; set; }

    [Parameter] public EventCallback<EditContext> OnValidSubmit { get; set; }

    [Parameter] public EventCallback<EditContext> OnInvalidSubmit { get; set; }

    [Parameter] public string? FormName { get; set; }

    [Parameter] public bool Enhance { get; set; }

    /// <summary>
    /// When set, submit awaits this instead of <see cref="EditContext.Validate"/>.
    /// <see cref="IsValidating"/> stays true until it returns. The callback fills the validation message store.
    /// </summary>
    [Parameter] public Func<EditContext, CancellationToken, Task<bool>>? ValidateAsync { get; set; }

    public bool IsModified => _state.IsModified;

    public bool IsSubmitting => _state.IsSubmitting;

    public bool IsValid => _state.IsValid;

    public bool IsValidating => _state.IsValidating;

    public bool HasErrors => _state.HasErrors;

    protected override void OnParametersSet()
    {
        if (Model is not null && EditContext is not null)
        {
            throw new InvalidOperationException($"{nameof(BaseFormRoot)} requires a Model parameter, or an EditContext parameter, but not both.");
        }

        if (Model is null && EditContext is null)
        {
            throw new InvalidOperationException($"{nameof(BaseFormRoot)} requires either a Model parameter, or an EditContext parameter.");
        }

        if (EditContext is not null)
        {
            _active = EditContext;
        }
        else if (_active is null || !ReferenceEquals(_active.Model, Model))
        {
            _active = new EditContext(Model!);
        }

        Subscribe(_active);
        Refresh();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<EditForm>(0);
        builder.AddComponentParameter(1, nameof(EditForm.EditContext), _active);
        builder.AddComponentParameter(2, nameof(EditForm.OnSubmit), EventCallback.Factory.Create<EditContext>(this, HandleSubmitAsync));
        builder.AddComponentParameter(3, nameof(EditForm.AdditionalAttributes), FormAttributes());
        builder.AddComponentParameter(4, nameof(EditForm.FormName), FormName);
        builder.AddComponentParameter(5, nameof(EditForm.Enhance), Enhance);
        builder.AddComponentParameter(6, nameof(EditForm.ChildContent), (RenderFragment<EditContext>)(_ => BuildBody));
        builder.CloseComponent();
    }

    private RenderFragment BuildBody => builder =>
    {
        builder.OpenComponent<CascadingValue<BaseFormState>>(0);
        builder.AddComponentParameter(1, nameof(CascadingValue<BaseFormState>.Value), _state);
        builder.AddComponentParameter(2, nameof(CascadingValue<BaseFormState>.ChildContent), (RenderFragment)(child =>
            child.AddContent(0, ChildContent?.Invoke(_state))));
        builder.CloseComponent();
    };

    private Dictionary<string, object> FormAttributes()
    {
        var attributes = new Dictionary<string, object>(StringComparer.Ordinal);
        if (AdditionalAttributes is not null)
        {
            foreach (var pair in AdditionalAttributes)
            {
                attributes[pair.Key] = pair.Value;
            }
        }

        attributes["data-state"] = _state.HasErrors ? "invalid" : "valid";
        SetPresence(attributes, "data-modified", _state.IsModified);
        SetPresence(attributes, "data-submitting", _state.IsSubmitting);
        SetPresence(attributes, "data-validating", _state.IsValidating);
        SetPresence(attributes, "data-invalid", _state.HasErrors);
        return attributes;
    }

    private static void SetPresence(Dictionary<string, object> attributes, string name, bool present)
    {
        if (present)
        {
            attributes[name] = string.Empty;
        }
        else
        {
            attributes.Remove(name);
        }
    }

    private async Task HandleSubmitAsync(EditContext context)
    {
        if (IsParameterSet(nameof(OnSubmit)))
        {
            _state.IsSubmitting = true;
            Refresh();
            await InvokeAsync(StateHasChanged);
            try
            {
                await OnSubmit.InvokeAsync(context);
            }
            finally
            {
                _state.IsSubmitting = false;
                Refresh();
                await InvokeAsync(StateHasChanged);
            }

            return;
        }

        _validation?.Cancel();
        _validation?.Dispose();
        _validation = new CancellationTokenSource();
        var token = _validation.Token;

        _state.IsSubmitting = true;
        _state.IsValidating = true;

        bool valid;
        try
        {
            valid = ValidateAsync is not null
                ? await ValidateAsync(context, token)
                : context.Validate();
        }
        finally
        {
            _state.IsValidating = false;
            Refresh();
        }

        if (!token.IsCancellationRequested)
        {
            if (valid && !_state.HasErrors)
            {
                await OnValidSubmit.InvokeAsync(context);
            }
            else
            {
                await OnInvalidSubmit.InvokeAsync(context);
            }
        }

        _state.IsSubmitting = false;
        Refresh();
        await InvokeAsync(StateHasChanged);
    }

    private void Subscribe(EditContext context)
    {
        if (ReferenceEquals(_subscribed, context))
        {
            return;
        }

        Unsubscribe();
        _subscribed = context;
        context.OnFieldChanged += HandleFieldChanged;
        context.OnValidationStateChanged += HandleValidationStateChanged;
    }

    private void Unsubscribe()
    {
        if (_subscribed is null)
        {
            return;
        }

        _subscribed.OnFieldChanged -= HandleFieldChanged;
        _subscribed.OnValidationStateChanged -= HandleValidationStateChanged;
        _subscribed = null;
    }

    private void HandleFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        Refresh();
        _ = InvokeAsync(StateHasChanged);
    }

    private void HandleValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        Refresh();
        _ = InvokeAsync(StateHasChanged);
    }

    private void Refresh()
    {
        if (_active is null)
        {
            return;
        }

        _state.EditContext = _active;
        _state.IsModified = _active.IsModified();
        _state.HasErrors = _active.GetValidationMessages().Any();
        _state.IsValid = !_state.HasErrors;
    }

    public void Dispose()
    {
        Unsubscribe();
        _validation?.Cancel();
        _validation?.Dispose();
        GC.SuppressFinalize(this);
    }
}
