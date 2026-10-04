using System.Globalization;

using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BaseRz.Core.Components.Slider;

/// <summary>
/// Single-thumb slider. Keyboard handling is on <see cref="BaseSliderThumb"/>; pointer tracking is
/// <c>_content/BaseRz.JS/slider.js</c>.
/// </summary>
public class BaseSliderRoot : BaseRzComponent, IAsyncDisposable
{
    public const string ModulePath = "./_content/BaseRz.JS/slider.js";

    private readonly ControllableState<decimal> _value = new();
    private readonly DotNetObjectReference<BaseSliderRoot> _self;
    private BaseSliderContext _context = default!;
    private IJSObjectReference? _module;
    private IJSObjectReference? _attachment;

    [Inject] private IJSRuntime Js { get; set; } = default!;

    public BaseSliderRoot()
    {
        _self = DotNetObjectReference.Create(this);
    }

    [Parameter] public decimal Value { get; set; }

    [Parameter] public EventCallback<decimal> ValueChanged { get; set; }

    [Parameter] public decimal DefaultValue { get; set; }

    [Parameter] public decimal Min { get; set; }

    [Parameter] public decimal Max { get; set; } = 100;

    [Parameter] public decimal Step { get; set; } = 1;

    [Parameter] public bool Disabled { get; set; }

    [Parameter] public Orientation Orientation { get; set; } = Orientation.Horizontal;

    protected override void OnInitialized()
    {
        _context = new BaseSliderContext(SetAsync, StepAsync, AttachTrackAsync);
    }

    protected override void OnParametersSet()
    {
        _value.Sync(IsParameterSet(nameof(Value)), Value, DefaultValue, ValueChanged);
        _context.Value = _value.Value;
        _context.Min = Min;
        _context.Max = Max;
        _context.Step = Step == 0 ? 1 : Step;
        _context.Disabled = Disabled;
        _context.Orientation = Orientation;
        _context.RangeStyle = RangeStyle();
        _context.ThumbStyle = ThumbStyle();
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddDataOrientation(2, Orientation);
        builder.AddDataDisabled(3, Disabled);

        builder.OpenComponent<CascadingValue<BaseSliderContext>>(4);
        builder.AddComponentParameter(5, nameof(CascadingValue<BaseSliderContext>.Value), _context);
        builder.AddComponentParameter(6, nameof(CascadingValue<BaseSliderContext>.ChildContent), ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }

    [JSInvokable]
    public Task SetRatio(double ratio)
    {
        if (Disabled)
        {
            return Task.CompletedTask;
        }

        var span = Max - Min;
        var next = Min + (decimal)ratio * span;
        return SetAsync(next);
    }

    internal decimal Clamp(decimal value)
    {
        var step = Step == 0 ? 1 : Step;
        if (value < Min)
        {
            value = Min;
        }

        if (value > Max)
        {
            value = Max;
        }

        var steps = Math.Round((value - Min) / step, MidpointRounding.AwayFromZero);
        value = Min + (steps * step);
        if (value > Max)
        {
            value = Max;
        }

        if (value < Min)
        {
            value = Min;
        }

        return value;
    }

    internal async Task SetAsync(decimal value)
    {
        if (Disabled)
        {
            return;
        }

        await _value.SetAsync(Clamp(value));
        _context.Value = _value.Value;
        StateHasChanged();
    }

    internal async Task StepAsync(int direction)
    {
        await SetAsync(_value.Value + ((Step == 0 ? 1 : Step) * direction));
    }

    internal static string Format(decimal value) => value.ToString(CultureInfo.InvariantCulture);

    internal string RangeStyle()
    {
        var percent = Percent();
        return Orientation == global::BaseRz.Core.Orientation.Vertical ? $"height:{percent}" : $"width:{percent}";
    }

    internal string ThumbStyle()
    {
        var percent = Percent();
        return Orientation == global::BaseRz.Core.Orientation.Vertical ? $"bottom:{percent}" : $"left:{percent}";
    }

    private string Percent()
    {
        var span = Max - Min;
        var ratio = span == 0 ? 0 : (double)((_value.Value - Min) / span);
        return (Math.Clamp(ratio, 0, 1) * 100).ToString("0", CultureInfo.InvariantCulture) + "%";
    }

    private async Task AttachTrackAsync(ElementReference track)
    {
        _module ??= await Js.InvokeAsync<IJSObjectReference>("import", ModulePath);
        if (_module is null)
        {
            return;
        }

        if (_attachment is not null)
        {
            await _attachment.InvokeVoidAsync("dispose");
        }

        _attachment = await _module.InvokeAsync<IJSObjectReference>(
            "attach",
            track,
            _self,
            DataAttributes.Of(Orientation));
    }

    public async ValueTask DisposeAsync()
    {
        if (_attachment is not null)
        {
            try
            {
                await _attachment.InvokeVoidAsync("dispose");
            }
            catch (JSDisconnectedException)
            {
            }
        }

        _self.Dispose();
        GC.SuppressFinalize(this);
    }
}
