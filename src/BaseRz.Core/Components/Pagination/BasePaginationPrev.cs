namespace BaseRz.Core.Components.Pagination;

/// <summary>Goes to the previous page; disabled on the first page.</summary>
public class BasePaginationPrev : BasePaginationStepButton
{
    protected override string DefaultLabel => "Previous page";

    protected override string DefaultText => "Previous";

    protected override int Step => -1;

    protected override bool CanStep(BasePaginationContext context) => context.CanGoPrevious;
}
