using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Utilities;

public static class DataAttributes
{
    public const string DataState = "data-state";
    public const string DataDisabled = "data-disabled";
    public const string DataOrientation = "data-orientation";
    public const string DataSide = "data-side";
    public const string DataAlign = "data-align";

    public const string Open = "open";
    public const string Closed = "closed";

    public static string OpenClosed(bool open) => open ? Open : Closed;

    public static string Of(Orientation orientation) => orientation switch
    {
        Orientation.Horizontal => "horizontal",
        Orientation.Vertical => "vertical",
        _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null),
    };

    public static string Of(Side side) => side switch
    {
        Side.Top => "top",
        Side.Right => "right",
        Side.Bottom => "bottom",
        Side.Left => "left",
        _ => throw new ArgumentOutOfRangeException(nameof(side), side, null),
    };

    public static string Of(Align align) => align switch
    {
        Align.Start => "start",
        Align.Center => "center",
        Align.End => "end",
        _ => throw new ArgumentOutOfRangeException(nameof(align), align, null),
    };

    public static string Of(Direction direction) => direction switch
    {
        Direction.Ltr => "ltr",
        Direction.Rtl => "rtl",
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
    };

    public static void AddDataState(this RenderTreeBuilder builder, int sequence, bool open) =>
        builder.AddAttribute(sequence, DataState, OpenClosed(open));

    /// <summary>Emits <c>data-disabled=""</c> only when <paramref name="disabled"/> is true.</summary>
    public static void AddDataDisabled(this RenderTreeBuilder builder, int sequence, bool disabled)
    {
        if (disabled)
        {
            builder.AddAttribute(sequence, DataDisabled, string.Empty);
        }
    }

    public static void AddDataOrientation(this RenderTreeBuilder builder, int sequence, Orientation orientation) =>
        builder.AddAttribute(sequence, DataOrientation, Of(orientation));

    public static void AddDataSide(this RenderTreeBuilder builder, int sequence, Side side) =>
        builder.AddAttribute(sequence, DataSide, Of(side));

    public static void AddDataAlign(this RenderTreeBuilder builder, int sequence, Align align) =>
        builder.AddAttribute(sequence, DataAlign, Of(align));
}
