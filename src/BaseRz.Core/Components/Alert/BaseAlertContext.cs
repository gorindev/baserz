namespace BaseRz.Core.Components.Alert;

/// <summary>Tracks the ids of the mounted Title and Description so the root can wire <c>aria-labelledby</c> / <c>aria-describedby</c>.</summary>
internal sealed class BaseAlertContext(Action changed)
{
    private object? _titleOwner;
    private object? _descriptionOwner;

    public string? TitleId { get; private set; }

    public string? DescriptionId { get; private set; }

    public void SetTitle(object owner, string? id) => Set(ref _titleOwner, owner, id, TitleId, value => TitleId = value);

    public void SetDescription(object owner, string? id) =>
        Set(ref _descriptionOwner, owner, id, DescriptionId, value => DescriptionId = value);

    public void ClearTitle(object owner)
    {
        if (ReferenceEquals(_titleOwner, owner))
        {
            SetTitle(owner, null);
            _titleOwner = null;
        }
    }

    public void ClearDescription(object owner)
    {
        if (ReferenceEquals(_descriptionOwner, owner))
        {
            SetDescription(owner, null);
            _descriptionOwner = null;
        }
    }

    private void Set(ref object? currentOwner, object owner, string? id, string? currentId, Action<string?> assign)
    {
        currentOwner = owner;
        if (string.Equals(currentId, id, StringComparison.Ordinal))
        {
            return;
        }

        assign(id);
        changed();
    }
}
