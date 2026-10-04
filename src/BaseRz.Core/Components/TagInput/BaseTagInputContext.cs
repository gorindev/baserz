namespace BaseRz.Core.Components.TagInput;

internal sealed class BaseTagInputContext(Func<string, Task> add, Func<string, Task> remove, Func<Task> removeLast)
{
    public IReadOnlyList<string> Values { get; set; } = [];

    public bool Disabled { get; set; }

    public Task AddAsync(string value) => add(value);

    public Task RemoveAsync(string value) => remove(value);

    public Task RemoveLastAsync() => removeLast();
}
