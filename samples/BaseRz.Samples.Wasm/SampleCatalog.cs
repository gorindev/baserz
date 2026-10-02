namespace BaseRz.Samples.Wasm;

public sealed record SampleEntry(string Name, string Route, string Summary);

public static class SampleCatalog
{
    public static IReadOnlyList<SampleEntry> P1 { get; } =
    [
        new("Alert", "components/alert", "role=\"alert\" with Title and Description wired to aria-labelledby / aria-describedby."),
        new("AspectRatio", "components/aspect-ratio", "Inline aspect-ratio from a w:h string."),
        new("Avatar", "components/avatar", "Image with a fallback that takes over on load error."),
        new("Breadcrumb", "components/breadcrumb", "Labeled nav with aria-current on the current page."),
        new("Button", "components/button", "Native or polymorphic button with disabled and loading states."),
        new("Image", "components/image", "Image with loading and error slots."),
        new("Label", "components/label", "Native label associated with a control."),
        new("Pagination", "components/pagination", "Page links with sibling/ellipsis range and prev/next."),
        new("Progress", "components/progress", "Determinate and indeterminate progressbar."),
        new("Separator", "components/separator", "Semantic or decorative separator, optionally labeled."),
        new("Table", "components/table", "Semantic table parts."),
        new("Toggle", "components/toggle", "Two-state button with aria-pressed."),
        new("VisuallyHidden", "components/visually-hidden", "Content for assistive technology only."),
    ];
}
