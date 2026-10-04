using Microsoft.AspNetCore.Components;

namespace BaseRz.Core.Components.SegmentedInput;

internal sealed class BaseSegmentedInputContext(Func<int, string, Task> set, Func<int, Task> clear)
{
    public string Value { get; set; } = "";

    public int Length { get; set; }

    public bool Disabled { get; set; }

    public string? Name { get; set; }

    public ElementReference[] Slots { get; set; } = [];

    public Task SetAsync(int index, string text) => set(index, text);

    public Task ClearAsync(int index) => clear(index);

    public async Task FocusAsync(int index)
    {
        if (index >= 0 && index < Slots.Length)
        {
            await Slots[index].FocusAsync();
        }
    }
}
