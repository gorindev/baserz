using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.Rating;

/// <summary>
/// <c>role="radiogroup"</c> of rating items. Hover and arrow keys preview; click, Enter, and Space commit.
/// </summary>
public class BaseRatingRoot : BaseRzComponent
{
    private readonly ControllableState<int> _value = new();
    private BaseRatingContext _context = default!;

    [Parameter] public int Value { get; set; }

    [Parameter] public EventCallback<int> ValueChanged { get; set; }

    [Parameter] public int DefaultValue { get; set; }

    [Parameter] public bool Disabled { get; set; }

    protected override void OnInitialized()
    {
        _context = new BaseRatingContext(CommitAsync, SetPreview);
    }

    protected override void OnParametersSet()
    {
        _value.Sync(IsParameterSet(nameof(Value)), Value, DefaultValue, ValueChanged);
        _context.Value = _value.Value;
        _context.Disabled = Disabled;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddAttribute(1, "role", "radiogroup");
        builder.AddMultipleAttributes(2, AdditionalAttributes);
        builder.AddDataDisabled(3, Disabled);
        builder.AddAttribute(4, "onmouseleave", EventCallback.Factory.Create(this, () => SetPreview(null)));

        builder.OpenComponent<CascadingValue<BaseRatingContext>>(5);
        builder.AddComponentParameter(6, nameof(CascadingValue<BaseRatingContext>.Value), _context);
        builder.AddComponentParameter(7, nameof(CascadingValue<BaseRatingContext>.ChildContent), ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }

    private async Task CommitAsync(int value)
    {
        if (Disabled)
        {
            return;
        }

        await _value.SetAsync(value);
        _context.Value = _value.Value;
        _context.Preview = null;
        StateHasChanged();
    }

    private void SetPreview(int? value)
    {
        if (Disabled || _context.Preview == value)
        {
            return;
        }

        _context.Preview = value;
        StateHasChanged();
    }
}
