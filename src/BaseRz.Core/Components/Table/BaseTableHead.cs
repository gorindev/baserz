namespace BaseRz.Core.Components.Table;

/// <summary>Header cell (<c>&lt;th&gt;</c>); pass <c>scope="col"</c> or <c>scope="row"</c> as an attribute.</summary>
public class BaseTableHead : BaseRzElementComponent
{
    protected override string DefaultElement => "th";
}
