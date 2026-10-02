namespace BaseRz.Core;

public enum ImageLoadingStatus
{
    Idle,
    Loading,
    Loaded,
    Error,
}

public static class ImageLoadingStatusExtensions
{
    /// <summary>Value emitted as <c>data-state</c>: <c>idle</c>, <c>loading</c>, <c>loaded</c>, or <c>error</c>.</summary>
    public static string ToDataState(this ImageLoadingStatus status) => status switch
    {
        ImageLoadingStatus.Idle => "idle",
        ImageLoadingStatus.Loading => "loading",
        ImageLoadingStatus.Loaded => "loaded",
        ImageLoadingStatus.Error => "error",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
    };

    internal static ImageLoadingStatus Initial(string? src) =>
        string.IsNullOrWhiteSpace(src) ? ImageLoadingStatus.Error : ImageLoadingStatus.Loading;
}
