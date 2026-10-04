using System.Linq.Expressions;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Form;

/// <summary>
/// One model property. Cascades the field id and validation state to <see cref="BaseFieldLabel"/>,
/// <see cref="BaseFieldControl"/>, <see cref="BaseFieldDescription"/>, and <see cref="BaseFieldErrorMessage"/>.
/// </summary>
public class BaseField<TValue> : BaseRzComponent
{
    private BaseFieldContext _context = default!;

    [CascadingParameter] public EditContext? EditContext { get; set; }

    [CascadingParameter] public BaseFormState? Form { get; set; }

    [Parameter, EditorRequired] public Expression<Func<TValue>>? For { get; set; }

    /// <summary>When null, <c>aria-required</c> follows a <c>[Required]</c> attribute on the property.</summary>
    [Parameter] public bool? Required { get; set; }

    protected override string IdPrefix => "baserz-field";

    protected override void OnInitialized()
    {
        _context = new BaseFieldContext(() => InvokeAsync(StateHasChanged));
    }

    protected override void OnParametersSet()
    {
        if (For is null)
        {
            throw new InvalidOperationException($"{nameof(BaseField<TValue>)} requires a {nameof(For)} parameter.");
        }

        var field = FieldIdentifier.Create(For);
        _context.Id = Id;
        _context.Field = field;
        _context.EditContext = EditContext;
        _context.IsValidating = Form?.IsValidating ?? false;
        _context.Required = Required ?? BaseFieldContext.HasRequiredAttribute(field);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);

        builder.OpenComponent<CascadingValue<BaseFieldContext>>(2);
        builder.AddComponentParameter(3, nameof(CascadingValue<BaseFieldContext>.Value), _context);
        builder.AddComponentParameter(4, nameof(CascadingValue<BaseFieldContext>.ChildContent), ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }
}
