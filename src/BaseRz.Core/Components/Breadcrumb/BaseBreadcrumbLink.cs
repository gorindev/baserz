namespace BaseRz.Core.Components.Breadcrumb;

/// <summary>Ancestor link; pass <c>href</c> as an attribute, or use <c>As</c> for another element.</summary>
public class BaseBreadcrumbLink : BaseRzElementComponent
{
    protected override string DefaultElement => "a";
}
