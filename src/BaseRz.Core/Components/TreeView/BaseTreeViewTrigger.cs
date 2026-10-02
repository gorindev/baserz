using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.TreeView;

/// <summary>
/// The item's label row. Names the tree item through <c>aria-labelledby</c>; a click selects the item and
/// toggles it when expandable.
/// </summary>
public class BaseTreeViewTrigger : BaseRzComponent, IDisposable
{
    [CascadingParameter] private BaseTreeViewNode? Node { get; set; }

    protected override void OnInitialized()
    {
        if (Node is not null && !Node.HasLabel)
        {
            Node.HasLabel = true;
            Node.Root.Changed();
        }
    }

    protected override void OnParametersSet() => Node?.LabelId.Use(AdditionalAttributes);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        if (Node is not null)
        {
            builder.AddAttribute(2, "id", Node.LabelId.Value);
            if (Node.HasContent)
            {
                builder.AddDataState(3, Node.Expanded);
            }

            if (Node.Selected)
            {
                builder.AddAttribute(4, "data-selected", string.Empty);
            }

            builder.AddDataDisabled(5, Node.Disabled);
        }

        builder.AddAttribute(6, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ActivateAsync));
        builder.AddContent(7, ChildContent);
        builder.CloseElement();
    }

    private async Task ActivateAsync()
    {
        if (Node is null || Node.Disabled)
        {
            return;
        }

        await Node.Root.SelectAsync(Node);
        await Node.Root.ToggleExpandedAsync(Node);
    }

    public void Dispose()
    {
        if (Node is not null)
        {
            Node.HasLabel = false;
        }

        GC.SuppressFinalize(this);
    }
}
