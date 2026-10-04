using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Form;

/// <summary>
/// <c>role="alert"</c> for the field's validation messages. <see cref="Mode"/> <see cref="BaseFieldErrorMode.All"/>
/// renders every message as a list. While messages (or child content) are showing, the alert id is part of the
/// field's <c>aria-describedby</c>.
/// </summary>
public class BaseFieldErrorMessage : BaseRzComponent, IDisposable
{
    [CascadingParameter] private BaseFieldContext? Context { get; set; }

    [Parameter] public BaseFieldErrorMode Mode { get; set; } = BaseFieldErrorMode.Single;

    protected override string DefaultElement => Mode == BaseFieldErrorMode.All ? "ul" : "div";

    private IReadOnlyList<string> Errors => Context?.Errors ?? [];

    private bool Visible => Errors.Count > 0 || ChildContent is not null;

    private string ErrorId => HasAttribute("id") || Context is null ? Id : $"{Context.Id}-error";

    protected override void OnParametersSet()
    {
        if (Context is null)
        {
            return;
        }

        if (Visible)
        {
            Context.ErrorMessageId.Set(this, ErrorId);
        }
        else
        {
            Context.ErrorMessageId.Clear(this);
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Visible)
        {
            return;
        }

        if (Mode == BaseFieldErrorMode.All && Errors.Count > 0)
        {
            builder.OpenElement(0, Element);
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "id", ErrorId);
            builder.AddAttribute(3, "role", "alert");
            for (var index = 0; index < Errors.Count; index++)
            {
                builder.OpenElement(4 + (index * 2), "li");
                builder.AddContent(5 + (index * 2), Errors[index]);
                builder.CloseElement();
            }

            builder.CloseElement();
            return;
        }

        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "id", ErrorId);
        builder.AddAttribute(3, "role", "alert");
        if (Errors.Count > 0)
        {
            builder.AddContent(4, Errors[0]);
        }
        else
        {
            builder.AddContent(4, ChildContent);
        }

        builder.CloseElement();
    }

    public void Dispose()
    {
        Context?.ErrorMessageId.Clear(this);
        GC.SuppressFinalize(this);
    }
}
