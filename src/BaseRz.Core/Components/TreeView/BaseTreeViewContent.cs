using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BaseRz.Core.Components.TreeView;

/// <summary>
/// Child items (<c>&lt;ul role="group"&gt;</c>). Mounting it makes the parent item expandable; its children are
/// rendered only while the parent is expanded.
/// </summary>
public class BaseTreeViewContent : BaseRzComponent, IDisposable
{
    [CascadingParameter] private BaseTreeViewNode? Node { get; set; }

    protected override string DefaultElement => "ul";

    protected override void OnInitialized()
    {
        if (Node is not null && !Node.HasContent)
        {
            Node.HasContent = true;
            Node.Root.Changed();
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Node is not null && !Node.Expanded)
        {
            return;
        }

        builder.OpenElement(0, Element);
        builder.AddAttribute(1, "role", "group");
        builder.AddMultipleAttributes(2, AdditionalAttributes);
        builder.AddDataState(3, true);
        builder.AddContent(4, ChildContent);
        builder.CloseElement();
    }

    public void Dispose()
    {
        if (Node is not null)
        {
            Node.HasContent = false;
        }

        GC.SuppressFinalize(this);
    }
}
