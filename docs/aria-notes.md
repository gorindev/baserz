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

---

## P2 — Disclosure and roving focus

Shared behavior for the composite widgets below:

- **Roving tabindex.** Accordion, Tabs, ToggleGroup, TreeView, and RadioGroup expose a single tab stop. The active item has `tabindex="0"` and the rest `-1`; arrow keys move it and call `FocusAsync` on the target. Disabled items are skipped. `Loop` (default `true`) wraps from the last item to the first; `Direction="Rtl"` swaps Left/Right in horizontal groups.
- **Disclosure content** (Collapsible, Accordion) stays in the DOM when closed with `hidden` and `inert`, so ids referenced by `aria-controls` always resolve. To animate, override `[hidden]` in CSS and key off `data-state="open|closed"`.
- **Ids.** Generated ids are derived from the root id (`{root}-trigger`, `{root}-content`, …). Passing `id` on a part overrides it and the paired `aria-controls` / `aria-labelledby` follows.
- **State** is controlled with `@bind-…` or uncontrolled with `Default…`; later `Default…` changes are ignored.
- **Form values.** Checkable controls are buttons with ARIA roles, never visible native checkbox or radio inputs. Setting `Name` adds an `<input type="hidden">` carrying the value while checked.
- **Known limits.** Without JS interop Blazor cannot conditionally `preventDefault` for single keys, so Space and arrow keys may also scroll a scrollable container. On native buttons the browser's own Enter activation still fires for radios and checkboxes (APG reserves Enter for forms).

## BaseCollapsible

Parts: `BaseCollapsibleRoot` (`div`), `BaseCollapsibleTrigger` (`button`), `BaseCollapsibleContent` (`div`).

- APG disclosure. Trigger has `aria-expanded` and `aria-controls` pointing at Content; Content is `role="region"` with `aria-labelledby` pointing at Trigger.
- `@bind-Open` / `DefaultOpen`. `Disabled` disables the trigger and ignores input.
- Root, Trigger, and Content carry `data-state="open|closed"`; `data-disabled` when disabled.
- Native trigger relies on the browser for Enter/Space; with `As` set to another element it gets `role="button"`, a tab stop, and Enter/Space toggling.

## BaseAccordion

Parts: `BaseAccordionRoot` (`div`), `BaseAccordionItem` (`div`), `BaseAccordionHeader` (`h3`), `BaseAccordionTrigger` (`button`), `BaseAccordionContent` (`div`).

- APG accordion. Each Trigger sits inside a Header, has `aria-expanded` and `aria-controls`; each Content is `role="region"` labeled by its Trigger.
- `Type="SelectionMode.Single"` (default) uses `@bind-Value` / `DefaultValue`; `SelectionMode.Multiple` uses `@bind-Values` / `DefaultValues`.
- `Collapsible` (default `true`) lets the open single item close. With `Collapsible="false"` the open trigger gets `aria-disabled="true"` because it cannot be closed.
- Up/Down (Left/Right when `Orientation="Horizontal"`), Home, and End move focus between triggers without toggling; Enter/Space toggle.
- Header defaults to `h3`; set `As` to fit the page's heading outline.
- `data-state="open|closed"` on Item, Header, Trigger, and Content; `data-disabled` on disabled items; `data-orientation` on Root and Trigger.

## BaseTabs

Parts: `BaseTabsRoot` (`div`), `BaseTabsList` (`div`), `BaseTabsTrigger` (`button`), `BaseTabsContent` (`div`).

- APG tabs with automatic activation: List is `role="tablist"` with `aria-orientation`; each Trigger is `role="tab"` with `aria-selected` and `aria-controls`; each Content is `role="tabpanel"`, `tabindex="0"`, labeled by its Trigger, and `hidden` when inactive.
- The selected tab is the tab stop. Left/Right (Up/Down when vertical), Home, and End move focus and select; disabled tabs are skipped.
- Label the List with `aria-label` or `aria-labelledby`.
- `data-state="active|inactive"` on Trigger and Content; `data-orientation` on Root, List, Trigger, and Content; `data-disabled` on disabled triggers.

## BaseToggleGroup

Parts: `BaseToggleGroupRoot` (`div`), `BaseToggleGroupItem` (`button`).

- Root is `role="group"`; label it with `aria-label` or `aria-labelledby`. Items are toggle buttons with `aria-pressed` and `data-state="on|off"`.
- `Type="SelectionMode.Single"` (default) uses `@bind-Value`; pressing the pressed item clears it. `SelectionMode.Multiple` uses `@bind-Values`.
- Arrow keys, Home, and End move focus only; Enter/Space toggle. `RovingFocus="false"` gives every item its own tab stop and disables arrow navigation.
- `data-orientation` on Root (default horizontal); `data-disabled` on disabled items.

## BaseTreeView

Parts: `BaseTreeViewRoot` (`ul`), `BaseTreeViewItem` (`li`), `BaseTreeViewTrigger` (`div`), `BaseTreeViewContent` (`ul`).

- APG tree view, single select. Root is `role="tree"` (label it); each Item is `role="treeitem"` with `aria-level`, `aria-selected`, and `aria-labelledby` pointing at its Trigger. Items with Content also get `aria-expanded` and `data-state="open|closed"`.
- Content is `role="group"` and is rendered only while its item is expanded.
- Down/Up move through visible items, Home/End to the first/last visible item; Right expands a closed branch or moves into an open one; Left collapses an open branch or moves to the parent; Enter/Space select. Clicking a Trigger selects and toggles.
- `@bind-Value` / `DefaultValue` for the selected item; `@bind-ExpandedValues` / `DefaultExpandedValues` for open branches.
- `data-selected` on the selected item; disabled items get `aria-disabled="true"` and `data-disabled` and are skipped.

## BaseRadioGroup

Parts: `BaseRadioGroupRoot` (`div`), `BaseRadioGroupItem` (`button`), `BaseRadioGroupIndicator` (`span`).

- APG radio group: Root is `role="radiogroup"` (label it), with `aria-required` and `aria-disabled` when set; Items are `role="radio"` with `aria-checked`.
- The checked item (or the first enabled one) is the tab stop. Arrow keys move focus and check the target; Space checks the focused item.
- `Orientation` is optional; when set it adds `aria-orientation` and `data-orientation`. Both arrow axes work either way.
- `@bind-Value` / `DefaultValue`. `Name` adds a hidden input with the checked value.
- Indicator renders only while its item is checked. `data-state="checked|unchecked"` on Item.

## BaseCheckbox

Parts: `BaseCheckboxRoot` (`button`), `BaseCheckboxIndicator` (`span`).

- APG checkbox: `role="checkbox"` with `aria-checked="true|false|mixed"` and `data-state="checked|unchecked|indeterminate"`.
- `@bind-Checked` / `DefaultChecked` and `@bind-Indeterminate` / `DefaultIndeterminate`. Activating an indeterminate checkbox clears indeterminate and checks it.
- Space toggles (native buttons also handle it themselves). `Required` sets `aria-required`.
- Inside a CheckboxGroup a checkbox with `Value` reads and writes the group's values instead of its own state.
- `Name` adds a hidden input with `Value` (default `on`) while checked. Indicator renders only while checked or indeterminate.

## BaseCheckboxGroup

Parts: `BaseCheckboxGroupRoot` (`div`), `BaseCheckboxGroupLabel` (`span`), `BaseCheckboxGroupDescription` (`div`), `BaseCheckboxGroupErrorMessage` (`div`).

- Root is `role="group"`. A mounted Label is referenced by `aria-labelledby` (unless the caller passes `aria-label`); Description and ErrorMessage are added to `aria-describedby`.
- A mounted ErrorMessage sets `aria-invalid="true"` on the root and has `role="alert"`; mount it when the error appears.
- `@bind-Values` / `DefaultValues`. `Disabled` disables every checkbox in the group; `Name` is used by checkboxes that do not set their own.

## BaseSwitch

Parts: `BaseSwitchRoot` (`button`), `BaseSwitchThumb` (`span`).

- APG switch: `role="switch"` with `aria-checked="true|false"`.
- `@bind-Checked` / `DefaultChecked`. Enter/Space toggle on non-native elements. `Required` sets `aria-required`.
- Root and Thumb carry `data-state="checked|unchecked"`; `data-disabled` when disabled.
- `Name` adds a hidden input with `Value` (default `on`) while on.

---

## P3 — Form and fields

## BaseForm

Parts: `BaseFormRoot` (`form`, via `EditForm`), `BaseField` (`div`), `BaseFieldLabel` (`label`), `BaseFieldControl` (`div`), `BaseFieldDescription` (`p`), `BaseFieldErrorMessage` (`div`, or `ul` when `Mode` is `All`).

- `BaseFormRoot` composes `EditForm` and cascades form flags: `IsModified`, `IsSubmitting`, `IsValid`, `IsValidating`, `HasErrors`. The `<form>` mirrors them with `data-state="valid|invalid"`, `data-modified`, `data-submitting`, `data-validating`, and `data-invalid`.
- Pass `Model` or `EditContext`, not both. `OnSubmit` skips automatic validation. Otherwise submit runs `EditContext.Validate()` (so `DataAnnotationsValidator` or any adapter subscribed to `OnValidationRequested` runs) and then `OnValidSubmit` or `OnInvalidSubmit`. `ValidateAsync` replaces `Validate()` when validation must be awaited; `IsValidating` is true for that call.
- `IsValid` is the absence of validation messages, including before the first submit.
- `BaseField For` identifies the model property. `BaseFieldControl` publishes the same flags for that field, plus `Id`, `DescribedBy`, `AriaInvalid`, and `AriaRequired` to its child content. `aria-required` follows `[Required]` or `Required="true"`.
- Description and error ids are added to `aria-describedby` only while those parts are mounted and, for the error, only while a message or child content is showing. Disposing a part drops its id.
- `BaseFieldErrorMessage Mode="Single"` (the default) is one `role="alert"` with the first message. `Mode="All"` is a `ul role="alert"` with one `li` per message, for rules such as a password policy.
- `BaseInput`, `BaseNumberField`, and, when a field is cascading, checkbox, radio group, and switch copy the field id and ARIA onto themselves and call `NotifyFieldChanged` after the value commits.

## BaseInput

Parts: `BaseInputRoot` (`input`).

- Native input. `@bind-Value` / `DefaultValue`. `Type` defaults to `text`.
- Inside a field, the input uses the field id and `aria-invalid`, `aria-describedby`, and `aria-required` unless the caller set them. Changing the value notifies `EditContext`.
- `data-disabled` when `Disabled`. Disabled and read-only inputs ignore edits.

## BaseNumberField

Parts: `BaseNumberFieldRoot` (`div`), `BaseNumberFieldGroup` (`div`), `BaseNumberFieldInput` (`input`), `BaseNumberFieldIncrement` (`button`), `BaseNumberFieldDecrement` (`button`).

- The input is `role="spinbutton"` with `aria-valuemin`, `aria-valuemax`, and `aria-valuenow` when it has a value. It is not `type="number"`.
- Up/Down add or subtract `Step`. Home and End set `Min` and `Max`. Values are clamped to that range and snapped to the step.
- Increment and Decrement are named "Increment" and "Decrement" unless the caller sets `aria-label`.
- `@bind-Value` / `DefaultValue` (`decimal?`). Notifies `EditContext` the same way an input does.

## BaseSlider

Parts: `BaseSliderRoot` (`div`), `BaseSliderTrack` (`div`), `BaseSliderRange` (`div`), `BaseSliderThumb` (`div`).

- The thumb is `role="slider"` with `aria-valuenow`, `aria-valuemin`, `aria-valuemax`, and `aria-orientation`. Root has `data-orientation`.
- Arrows step; Home and End jump to min and max. Disabled thumbs are not in the tab sequence.
- Track and range are `aria-hidden`. Range size is an inline `width` or `height`.
- Pointer capture is `slider.js` at `_content/BaseRz.JS/slider.js`.

## BaseRating

Parts: `BaseRatingRoot` (`div`), `BaseRatingItem` (`button`).

- Root is `role="radiogroup"`. Each item is `role="radio"` with `aria-checked` for the committed value only.
- Hover and arrow keys set `data-state="preview"` and do not change `Value`. Click, Enter, or Space commits. The committed item uses `data-state="checked"`.

## BaseTagInput

Parts: `BaseTagInputRoot` (`div`), `BaseTagInputInput` (`input`), `BaseTagInputTag` (`span`), `BaseTagInputTagRemove` (`button`).

- Enter adds the input text as a tag and clears the input. Backspace on an empty input removes the last tag.
- TagRemove's accessible name defaults to "Remove {value}".
- `@bind-Values` / `DefaultValues`.

## BaseSegmentedInput

Parts: `BaseSegmentedInputRoot` (`div`), `BaseSegmentedInputGroup` (`div`), `BaseSegmentedInputSlot` (`input`), `BaseSegmentedInputSeparator` (`span`).

- Slots share one string value. A character advances focus; paste fills from the focused slot; Backspace clears the slot or the previous one.
- Group is `role="group"`. Separator is `aria-hidden`.
- `Name` adds a hidden input with the combined value.
