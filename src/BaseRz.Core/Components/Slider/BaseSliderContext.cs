using Microsoft.AspNetCore.Components;

namespace BaseRz.Core.Components.Slider;

internal sealed class BaseSliderContext(Func<decimal, Task> set, Func<int, Task> step, Func<ElementReference, Task> attach)
{
    public decimal Value { get; set; }

    public decimal Min { get; set; }

    public decimal Max { get; set; }

    public decimal Step { get; set; }

    public bool Disabled { get; set; }

    public Orientation Orientation { get; set; }

    public string? RangeStyle { get; set; }

    public string? ThumbStyle { get; set; }

    public Task SetAsync(decimal value) => set(value);

    public Task StepAsync(int direction) => step(direction);

    public Task AttachTrackAsync(ElementReference track) => attach(track);
}
