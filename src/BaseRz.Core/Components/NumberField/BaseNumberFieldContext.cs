namespace BaseRz.Core.Components.NumberField;

internal sealed class BaseNumberFieldContext(Func<decimal, Task> set, Func<int, Task> step)
{
    public decimal? Value { get; set; }

    public decimal Min { get; set; }

    public decimal Max { get; set; }

    public decimal Step { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool Required { get; set; }

    public string? Name { get; set; }

    public Task SetAsync(decimal value) => set(value);

    public Task StepAsync(int direction) => step(direction);
}
