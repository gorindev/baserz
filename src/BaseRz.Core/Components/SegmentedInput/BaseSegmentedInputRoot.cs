using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.SegmentedInput;

/// <summary>OTP/PIN value shared by <see cref="BaseSegmentedInputSlot"/> parts. <see cref="Name"/> posts a hidden input.</summary>
public class BaseSegmentedInputRoot : BaseRzComponent
{
    private readonly ControllableState<string> _value = new(StringComparer.Ordinal);
    private BaseSegmentedInputContext _context = default!;

    [Parameter] public string? Value { get; set; }

    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    [Parameter] public string? DefaultValue { get; set; }

    [Parameter] public int Length { get; set; } = 6;

    [Parameter] public bool Disabled { get; set; }

    [Parameter] public string? Name { get; set; }

    protected override void OnInitialized()
    {
        _context = new BaseSegmentedInputContext(SetAsync, ClearAsync);
    }

    protected override void OnParametersSet()
    {
        _value.Sync(IsParameterSet(nameof(Value)), Value ?? "", DefaultValue ?? "", ValueChanged);
        _context.Value = _value.Value ?? "";
        _context.Length = Math.Max(1, Length);
        _context.Disabled = Disabled;
        _context.Name = Name;
        if (_context.Slots.Length != _context.Length)
        {
            _context.Slots = new ElementReference[_context.Length];
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddDataDisabled(2, Disabled);

        builder.OpenComponent<CascadingValue<BaseSegmentedInputContext>>(3);
        builder.AddComponentParameter(4, nameof(CascadingValue<BaseSegmentedInputContext>.Value), _context);
        builder.AddComponentParameter(5, nameof(CascadingValue<BaseSegmentedInputContext>.ChildContent), ChildContent);
        builder.CloseComponent();
        builder.AddHiddenInput(6, Name, _context.Value);
        builder.CloseElement();
    }

    private async Task SetAsync(int index, string text)
    {
        if (Disabled || index < 0 || index >= _context.Length)
        {
            return;
        }

        var chars = Chars();
        var written = 0;
        foreach (var ch in text)
        {
            if (index + written >= chars.Length)
            {
                break;
            }

            chars[index + written] = ch;
            written++;
        }

        await CommitAsync(chars);
        if (written > 0)
        {
            await _context.FocusAsync(Math.Min(index + written, _context.Length - 1));
        }
    }

    private async Task ClearAsync(int index)
    {
        if (Disabled || index < 0 || index >= _context.Length)
        {
            return;
        }

        var chars = Chars();
        if (chars[index] != '\0')
        {
            chars[index] = '\0';
            await CommitAsync(chars);
            return;
        }

        var previous = index - 1;
        if (previous >= 0)
        {
            chars[previous] = '\0';
            await CommitAsync(chars);
            await _context.FocusAsync(previous);
        }
    }

    private char[] Chars()
    {
        var chars = new char[_context.Length];
        var current = _value.Value ?? "";
        for (var i = 0; i < chars.Length && i < current.Length; i++)
        {
            chars[i] = current[i];
        }

        return chars;
    }

    private async Task CommitAsync(char[] chars)
    {
        var text = new string(chars).TrimEnd('\0');
        await _value.SetAsync(text);
        _context.Value = text;
        StateHasChanged();
    }
}
