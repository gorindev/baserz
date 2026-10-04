using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Form;

/// <summary>
/// Wraps the input and the field description/error. Publishes field flags
/// (<see cref="BaseFieldControlState.IsModified"/>, <see cref="BaseFieldControlState.IsValid"/>,
/// <see cref="BaseFieldControlState.IsValidating"/>, <see cref="BaseFieldControlState.HasErrors"/>)
/// as <c>data-*</c> attributes and to child content.
/// </summary>
public class BaseFieldControl : BaseRzTemplatedComponent<BaseFieldControlState>, IDisposable
{
    private EditContext? _subscribed;

    [CascadingParameter] private BaseFieldContext? Context { get; set; }

    [CascadingParameter] private EditContext? EditContext { get; set; }

    protected override void OnParametersSet()
    {
        if (EditContext is null || ReferenceEquals(_subscribed, EditContext))
        {
            return;
        }

        Unsubscribe();
        _subscribed = EditContext;
        EditContext.OnFieldChanged += HandleChanged;
        EditContext.OnValidationStateChanged += HandleChanged;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var state = Context?.ToControlState() ?? new BaseFieldControlState { Id = Id, IsValid = true };

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "data-state", state.HasErrors ? "invalid" : "valid");
        if (state.IsModified)
        {
            builder.AddAttribute(3, "data-modified", string.Empty);
        }

        if (state.IsValidating)
        {
            builder.AddAttribute(4, "data-validating", string.Empty);
        }

        if (state.HasErrors)
        {
            builder.AddAttribute(5, "data-invalid", string.Empty);
        }

        builder.AddContent(6, ChildContent?.Invoke(state));
        builder.CloseElement();
    }

    private void HandleChanged(object? sender, EventArgs e) => Context?.Changed();

    private void Unsubscribe()
    {
        if (_subscribed is null)
        {
            return;
        }

        _subscribed.OnFieldChanged -= HandleChanged;
        _subscribed.OnValidationStateChanged -= HandleChanged;
        _subscribed = null;
    }

    public void Dispose()
    {
        Unsubscribe();
        GC.SuppressFinalize(this);
    }
}
