# ARIA notes

Roles, states, keyboard behavior, and styling hooks for each component. Part names follow [components.md](components.md). Every part passes unmatched attributes through and emits only the caller's `class`; every part accepts `As` to change its element.

---

## P1 — Static and simple primitives

## BaseAlert

Parts: `BaseAlertRoot` (`div`), `BaseAlertTitle` (`h5`), `BaseAlertDescription` (`div`).

- Root has `role="alert"`, so its content is announced when it is inserted. Mount it when the message appears rather than toggling its visibility.
- A mounted Title is referenced by `aria-labelledby` and a mounted Description by `aria-describedby`. Ids are generated unless the caller passes `id` on the part. Removing a part removes the reference.
- Title defaults to `h5`; set `As` to fit the page's heading outline, or to a non-heading element.

## BaseAspectRatio

Parts: `BaseAspectRatioRoot` (`div`).

- No role. `Ratio="w:h"` becomes inline `aspect-ratio: w / h`, followed by the caller's `style`. Invalid or non-positive ratios fall back to `1 / 1`.
- `data-ratio` carries the parsed ratio.

## BaseAvatar

Parts: `BaseAvatarRoot` (`span`), `BaseAvatarImage` (`img`), `BaseAvatarFallback` (`span`).

- Image takes `Src` and `Alt` (default empty, which makes it decorative). Give it a meaningful `Alt` when the avatar identifies someone.
- Root and Image expose `data-state="idle|loading|loaded|error"`. The image is in the DOM while loading so the browser can fetch it; hide it with `img[data-state="loading"]` if needed.
- Fallback renders until the image loads. While the image is loading it is `aria-hidden="true"` so the image's alt text is the only name; on error (or with no `Src`) it is exposed normally. A new `Src` restarts loading.

## BaseBreadcrumb

Parts: `BaseBreadcrumbRoot` (`nav`), `BaseBreadcrumbList` (`ol`), `BaseBreadcrumbItem` (`li`), `BaseBreadcrumbLink` (`a`), `BaseBreadcrumbPage` (`span`), `BaseBreadcrumbSeparator` (`li`).

- Follows the APG breadcrumb pattern. Root is a navigation landmark labeled "Breadcrumb" unless the caller passes `aria-label` or `aria-labelledby`.
- Page is the current location: `role="link"`, `aria-disabled="true"`, `aria-current="page"`.
- Separator is `role="presentation"` and `aria-hidden="true"`; it renders `/` unless given content.
- Pass `href` to Link as a normal attribute.

## BaseButton

Parts: `BaseButtonRoot` (`button`).

- Native `<button>` by default, with `type` from `Type` (default `button`) and native `disabled`.
- With `As` set to another element: `role="button"`, `tabindex` (from `TabIndex`, `-1` when disabled), `aria-disabled="true"` when disabled, and Enter/Space invoke `OnClick`. A native button relies on the browser's own key handling.
- `Loading` keeps the button focusable but blocks activation: `aria-busy="true"`, `aria-disabled="true"`, `data-loading`.
- `data-disabled` when disabled.
- Known limit: on non-native elements Space may also scroll the page, since Blazor cannot conditionally prevent the default for one key.

## BaseImage

Parts: `BaseImageRoot` (`img`).

- The `img` carries `data-state="loading|loaded"` and `OnLoadingStatusChange` reports transitions.
- `LoadingContent` renders next to the image while it loads. On error, or with no `Src`, the image is removed and `ErrorContent` renders instead. Make `ErrorContent` meaningful text when the image conveyed information.
- Unlike Avatar there is no shared fallback cascade.

## BaseLabel

Parts: `BaseLabelRoot` (`label`).

- Native label; `For` sets `for` to associate a control by id. Wrapping the control inside the label also works.

## BasePagination

Parts: `BasePaginationRoot` (`nav`), `BasePaginationList` (`ul`), `BasePaginationItem` (`li`), `BasePaginationLink` (`button`), `BasePaginationPrev` (`button`), `BasePaginationNext` (`button`), `BasePaginationEllipsis` (`span`).

- Root is a navigation landmark labeled "Pagination" unless the caller labels it. It owns the page (`@bind-Page` or `DefaultPage`) and passes a `BasePaginationContext` to its child content.
- `context.Items` is the page range: first page, `SiblingCount` pages either side of the current page, and the last page, with `null` for each gap. A gap of exactly one page shows that page instead of an ellipsis.
- Link: `aria-current="page"` and `data-state="active"` on the current page, `data-state="inactive"` otherwise. Renders the page number unless given content.
- Prev/Next: `aria-label` "Previous page" / "Next page" (overridable), disabled at the first/last page.
- Ellipsis is `aria-hidden="true"` and renders `…` unless given content.
- `Disabled` disables every button and sets `data-disabled` on the root and buttons.

## BaseProgress

Parts: `BaseProgressRoot` (`div`), `BaseProgressIndicator` (`div`).

- Root: `role="progressbar"`, `aria-valuemin` (`Min`, default 0), `aria-valuemax` (`Max`, default 100), and `aria-valuenow` when `Value` is set. The value is clamped to the range.
- A null `Value` is indeterminate: `aria-valuenow` is omitted.
- `GetValueLabel(value, max)` supplies `aria-valuetext`.
- Both parts carry `data-state="indeterminate|loading|complete"`, `data-value` (when determinate), and `data-max`. The indicator has no size of its own; set its width or transform yourself.
- Give the root an accessible name with `aria-label` or `aria-labelledby`.

## BaseSeparator

Parts: `BaseSeparatorRoot` (`div`).

- `role="separator"`, with `aria-orientation="vertical"` only when vertical (horizontal is the ARIA default).
- `Decorative` uses `role="none"` and omits `aria-orientation`.
- `data-orientation="horizontal|vertical"` in all modes.
- Labeled mode: with child content it renders an empty `aria-hidden` span on each side of the content so the caller can draw the two line segments. A separator's children are presentational, so the label text is visual only; use a heading if it must be announced.

## BaseTable

Parts: `BaseTableRoot` (`table`), `BaseTableHeader` (`thead`), `BaseTableBody` (`tbody`), `BaseTableFooter` (`tfoot`), `BaseTableRow` (`tr`), `BaseTableHead` (`th`), `BaseTableCell` (`td`), `BaseTableCaption` (`caption`).

- Native table semantics only. Pass `scope="col"` / `scope="row"` on `BaseTableHead`. The caption names the table.
- No sorting or selection in v1.

## BaseToggle

Parts: `BaseToggleRoot` (`button`).

- APG button with `aria-pressed="true|false"` and `data-state="on|off"`.
- State is controlled with `@bind-Pressed` or uncontrolled with `DefaultPressed`; later `DefaultPressed` changes are ignored.
- Native `<button type="button">` handles Enter/Space itself. With `As` set to another element it gets `role="button"`, a tab stop, and Enter/Space toggling.
- `Disabled` sets native `disabled` (or `aria-disabled` on non-native elements) and `data-disabled`, and ignores input.

## BaseVisuallyHidden

Parts: `BaseVisuallyHiddenRoot` (`span`).

- Hides content visually while keeping it in the accessibility tree: an inline clip style (`position: absolute`, 1px box, `overflow: hidden`, `clip: rect(0, 0, 0, 0)`, `white-space: nowrap`), followed by the caller's `style`.
- The style is inline because BaseRz emits no classes of its own and `baserz-reset.css` is opt-in.
- Never uses `display: none`, `visibility: hidden`, or `aria-hidden`, all of which would remove the content from assistive technology.
