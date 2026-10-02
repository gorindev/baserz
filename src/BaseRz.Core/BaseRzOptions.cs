namespace BaseRz.Core;

public sealed class BaseRzOptions
{
    /// <summary>Reading direction used by keyboard navigation when a component has no explicit <c>Dir</c>.</summary>
    public Direction DefaultDirection { get; set; } = Direction.Ltr;
}
