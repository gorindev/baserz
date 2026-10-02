namespace BaseRz.Core.Components.Pagination;

/// <summary>Goes to the next page; disabled on the last page.</summary>
public class BasePaginationNext : BasePaginationStepButton
{
    protected override string DefaultLabel => "Next page";

    protected override string DefaultText => "Next";

    protected override int Step => 1;

    protected override bool CanStep(BasePaginationContext context) => context.CanGoNext;
}
