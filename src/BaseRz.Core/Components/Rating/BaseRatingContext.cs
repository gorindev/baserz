namespace BaseRz.Core.Components.Rating;

internal sealed class BaseRatingContext(Func<int, Task> commit, Action<int?> preview)
{
    public int Value { get; set; }

    public int? Preview { get; set; }

    public bool Disabled { get; set; }

    public Task CommitAsync(int value) => commit(value);

    public void SetPreview(int? value) => preview(value);
}
