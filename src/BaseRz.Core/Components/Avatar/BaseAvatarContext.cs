namespace BaseRz.Core.Components.Avatar;

/// <summary>Loading status reported by <see cref="BaseAvatarImage"/> and read by <see cref="BaseAvatarFallback"/>.</summary>
internal sealed class BaseAvatarContext(Action changed)
{
    public ImageLoadingStatus Status { get; private set; } = ImageLoadingStatus.Idle;

    public void SetStatus(ImageLoadingStatus status)
    {
        if (Status == status)
        {
            return;
        }

        Status = status;
        changed();
    }
}
