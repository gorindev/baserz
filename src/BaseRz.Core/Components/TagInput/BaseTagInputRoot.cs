using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.TagInput;

/// <summary>Collection of tags edited from <see cref="BaseTagInputInput"/>.</summary>
public class BaseTagInputRoot : BaseRzComponent
{
    private readonly ControllableState<IReadOnlyList<string>> _values = new(SequenceComparer.Instance);
    private BaseTagInputContext _context = default!;

    [Parameter] public IReadOnlyList<string>? Values { get; set; }

    [Parameter] public EventCallback<IReadOnlyList<string>> ValuesChanged { get; set; }

    [Parameter] public IReadOnlyList<string>? DefaultValues { get; set; }

    [Parameter] public bool Disabled { get; set; }

    protected override void OnInitialized()
    {
        _context = new BaseTagInputContext(AddAsync, RemoveAsync, RemoveLastAsync);
    }

    protected override void OnParametersSet()
    {
        _values.Sync(IsParameterSet(nameof(Values)), Values ?? [], DefaultValues ?? [], ValuesChanged);
        _context.Values = _values.Value ?? [];
        _context.Disabled = Disabled;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddDataDisabled(2, Disabled);

        builder.OpenComponent<CascadingValue<BaseTagInputContext>>(3);
        builder.AddComponentParameter(4, nameof(CascadingValue<BaseTagInputContext>.Value), _context);
        builder.AddComponentParameter(5, nameof(CascadingValue<BaseTagInputContext>.ChildContent), ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }

    private async Task AddAsync(string value)
    {
        if (Disabled || string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        var next = _context.Values.Append(value.Trim()).ToArray();
        await _values.SetAsync(next);
        _context.Values = next;
        StateHasChanged();
    }

    private async Task RemoveAsync(string value)
    {
        if (Disabled)
        {
            return;
        }

        var list = _context.Values.ToList();
        if (list.Remove(value))
        {
            await _values.SetAsync(list);
            _context.Values = list;
            StateHasChanged();
        }
    }

    private Task RemoveLastAsync()
    {
        var last = _context.Values.LastOrDefault();
        return last is null ? Task.CompletedTask : RemoveAsync(last);
    }

    private sealed class SequenceComparer : IEqualityComparer<IReadOnlyList<string>>
    {
        public static readonly SequenceComparer Instance = new();

        public bool Equals(IReadOnlyList<string>? x, IReadOnlyList<string>? y) =>
            x is not null && y is not null && x.SequenceEqual(y);

        public int GetHashCode(IReadOnlyList<string> obj) => obj.Count;
    }
}
