using System.Globalization;

using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Core.Components.TreeView;

/// <summary>
/// <c>role="treeitem"</c> for <see cref="Value"/>. Carries focus (roving <c>tabindex</c>), <c>aria-level</c>,
/// <c>aria-selected</c>, and <c>aria-expanded</c> when it has a <see cref="BaseTreeViewContent"/>.
/// </summary>
public class BaseTreeViewItem : BaseRzComponent, IDisposable
{
    private BaseTreeViewNode? _node;
    private ElementReference _element;

    [CascadingParameter] private BaseTreeViewContext? Context { get; set; }

    [CascadingParameter] private BaseTreeViewNode? Parent { get; set; }

    [Parameter, EditorRequired] public string Value { get; set; } = string.Empty;

    [Parameter] public bool Disabled { get; set; }

    protected override string DefaultElement => "li";

    protected override string IdPrefix => "baserz-tree-item";

    protected override void OnInitialized()
    {
        if (Context is not null)
        {
            _node = new BaseTreeViewNode(Context, Parent, this, Id, () => _element.FocusAsync().AsTask());
            Context.Add(_node);
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddAttribute(1, "role", "treeitem");
        builder.AddMultipleAttributes(2, AdditionalAttributes);
        if (_node is not null)
        {
            builder.AddAttribute(3, "tabindex", _node.Root.TabIndexFor(_node));
            builder.AddAttribute(4, "aria-level", _node.Level.ToString(CultureInfo.InvariantCulture));
            builder.AddAttribute(5, "aria-selected", _node.Selected ? "true" : "false");
            if (_node.HasContent)
            {
                builder.AddAttribute(6, "aria-expanded", _node.Expanded ? "true" : "false");
                builder.AddDataState(7, _node.Expanded);
            }

            if (_node.HasLabel)
            {
                builder.AddAttribute(8, "aria-labelledby", _node.LabelId.Value);
            }

            if (_node.Selected)
            {
                builder.AddAttribute(9, "data-selected", string.Empty);
            }
        }

        if (Disabled)
        {
            builder.AddAttribute(10, "aria-disabled", "true");
        }

        builder.AddDataDisabled(11, Disabled);
        builder.AddAttribute(12, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDownAsync));
        builder.AddEventStopPropagationAttribute(13, "onkeydown", true);
        builder.AddElementReferenceCapture(14, element => _element = element);

        builder.OpenComponent<CascadingValue<BaseTreeViewNode?>>(15);
        builder.AddComponentParameter(16, "Value", _node);
        builder.AddComponentParameter(17, "ChildContent", ChildContent);
        builder.CloseComponent();
        builder.CloseElement();
    }

    private Task HandleKeyDownAsync(KeyboardEventArgs args) =>
        _node is null ? Task.CompletedTask : _node.Root.HandleKeyDownAsync(_node, args);

    public void Dispose()
    {
        if (_node is not null)
        {
            _node.Root.Remove(_node);
        }

        GC.SuppressFinalize(this);
    }
}
