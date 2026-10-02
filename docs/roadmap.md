# Roadmap

Phased plan to implement BaseRz as an unstyled, accessible, headless UI library for Blazor (WebAssembly, Server) and .NET MAUI Blazor Hybrid. Work lives in `src/BaseRz.Core`, isolated JS in `src/BaseRz.JS`, tests in `tests/BaseRz.Tests`, and demos in `samples/BaseRz.Samples.Wasm`.

Component names and parts are canonical in [components.md](components.md). Where ShadRz already has working behavior mixed with Tailwind, this roadmap ports the headless logic and strips styles.

## Goals and scope

- **Headless:** no opinionated CSS. Components emit ARIA, `data-*` state, and the caller's `class` only.
- **WAI-ARIA / APG:** standard roles, states, and keyboard behavior out of the box.
- **Cross-platform:** WASM, Server, and MAUI Hybrid WebViews.
- **Lightweight JS:** focus trap, popover positioning, dismiss layers, and pointer tracking live in `BaseRz.JS`.
- **Opt-in reset:** `baserz-reset.css` as described in the README.

**Out of scope:** ShadRz layouts (Split, Docked, Grid, Flex, Canvas), Sidebar, design-system chrome (Card, Typo, List, ButtonAppearance), and rewriting ShadRz to consume BaseRz.

## Conventions

Every component in [components.md](components.md) follows this contract.

| Topic | Rule |
| --- | --- |
| Naming | Public types match [components.md](components.md), e.g. `BaseCollapsibleRoot`. Namespace `BaseRz.Core.Components.<Name>`. |
| Compound parts | Root owns cascading context; parts consume it. Do not flatten parts into a single component. |
| State | Controlled + uncontrolled: `Open` / `OpenChanged` / `DefaultOpen` and `Value` / `ValueChanged` / `DefaultValue`, via shared `ControllableState<T>`. |
| Data attributes | `data-state`, `data-disabled`, `data-orientation`, `data-side`, `data-align` where they apply. |
| Attributes | Unmatched attributes pass through. The only `class` emitted is the caller's (`Css.CallerClass`). |
| Ids | Generated ids wire `aria-controls`, `aria-labelledby`, `aria-describedby`. |
| Polymorphism | `As` (or equivalent) to change the rendered element when APG allows it. |
| JS | Isolated modules under `BaseRz.JS` / `_content/BaseRz.JS/…`. No inline scripts. |

ShadRz today is **controlled-only** (mutates `[Parameter]` + `*Changed`, no `Default*`). BaseRz must add uncontrolled mode.

README Quick Start currently uses `CollapsibleRoot` and `@bind-IsOpen`. Align README to `BaseCollapsibleRoot` and `@bind-Open` when P2 lands (see P8).

## Definition of done (per component)

A component is done when all of the following are true:

- [ ] Every part listed under that heading in [components.md](components.md) exists.
- [ ] Roles, properties, and keyboard map match the relevant WAI-ARIA APG pattern.
- [ ] Controlled and uncontrolled modes both work (where the component has open/value state).
- [ ] bUnit tests cover rendered ARIA, keyboard handling, and state changes (see TDD below).
- [ ] `samples/BaseRz.Samples.Wasm` has a page exercising the compound API.
- [ ] Docs include an ARIA notes section for the component.

## TDD

Work is **test-first**. Do not implement a phase item until its tests exist and fail for the right reason.

1. Add the tests listed under that phase (**red**).
2. Implement until `dotnet test tests/BaseRz.Tests` is **green**.
3. Run the phase **Checks** gate. A failed check blocks the next phase.

**Homes:** xunit + bUnit in `tests/BaseRz.Tests/` (already referenced). JS modules get tests next to the source in `src/BaseRz.JS` (or a small Node runner if a .NET host cannot execute them). Replace `UnitTest1` in P0.

**Default red suite for every public component** (add besides the phase-specific cases):

| Test | Assert |
| --- | --- |
| `Parts_Render` | Every part in [components.md](components.md) is present in the tree. |
| `CallerClass_Only` | Root/`class` equals the unmatched `class` attribute; **no** Tailwind/token class strings (`flex`, `bg-`, `text-`, `rounded-`, `p-`, `gap-`, `border-`, `shadow-`, `hover:`, `data-[`). |
| `DataState_And_Disabled` | `data-state` / `data-disabled` match parameters where they apply. |
| `UnmatchedAttributes_PassThrough` | `id`, `data-*`, `aria-*` from the caller appear on the host element. |
| `Controlled_Bind` | `Open`/`Value`/`Pressed` two-way bind; parent wins on re-render. |
| `Uncontrolled_Default` | `Default*` seeds state; later `Default*` changes do not reset. |
| `Disabled_IgnoresInput` | Clicks/keys do not change state when disabled. |

Keyboard tests use bUnit `KeyboardEventDispatchHelper` (or the P0 helper). Overlay/JS tests stub `IJSRuntime` unless noted.

---

## P0 — Foundation

No public components. Unblocks every later phase.

Current tree is Razor class-library templates (`Component1.razor`, `ExampleJsInterop`). Replace them.

- [x] Remove template Razor/JS samples from `BaseRz.Core` and `BaseRz.JS`.
- [x] Port `Css` as public `BaseRz.Core.Utilities.Css` — `Join`, `CallerClass`, `AttributesWithoutClass`. **Port from:** `ShadRz/Base/Css.cs`.
- [x] Add `ControllableState<T>` (controlled vs `Default*` uncontrolled, `*Changed` callbacks).
- [x] Add id generator and helpers that apply `data-state` / `data-disabled` / `data-orientation` / `data-side` / `data-align`.
- [x] Add a shared component base: `AdditionalAttributes`, `ChildContent`, `As`.
- [x] Add shared enums: `Orientation`, `Side`, `Align`, `Direction` (placement later extends `Side` + `Align`).
- [x] Add `IServiceCollection.AddBaseRz()` (overlay/toast services registered here; hosts must still render hosts from P4).
- [x] Add opt-in `wwwroot/css/baserz-reset.css` and document the README `<link>`.
- [x] Add bUnit test helpers (render with cascading values, keyboard dispatch).
- [x] Replace the WASM sample shell (nav + placeholder pages). Keep Bootstrap in the sample only as demo chrome, not as a BaseRz dependency.

### Tests (write first)

- [x] `CssTests.Join_SkipsNullAndWhitespace`
- [x] `CssTests.CallerClass_ReadsClass_RemovesNothingFromSource`
- [x] `CssTests.AttributesWithoutClass_ReturnsCopyWithoutClass`
- [x] `ControllableStateTests.Uncontrolled_UsesDefault_ThenIgnoresDefaultChanges`
- [x] `ControllableStateTests.Controlled_EmitsChanged_AndDoesNotMutateStaleParent`
- [x] `IdGeneratorTests.IsStablePerInstance_AndUniqueAcrossInstances`
- [x] `DataAttributeTests.MapsOpenClosed_Disabled_Orientation_Side_Align`
- [x] `ServiceCollectionTests.AddBaseRz_RegistersRequiredServices`
- [x] `ResetCssTests.BaserzReset_IsPackagedAsStaticWebAsset`
- [x] Delete `UnitTest1`; helpers compile and are used by at least one test.

### Checks

- [x] `dotnet test tests/BaseRz.Tests` green; `dotnet build BaseRz.slnx` green.
- [x] No `Component1.razor` / `ExampleJsInterop` left in Core or JS.
- [x] `baserz-reset.css` reachable as `_content/BaseRz.Core/css/baserz-reset.css`.
- [x] Public API of `Css` matches `Join` / `CallerClass` / `AttributesWithoutClass`.

---

## P1 — Static and simple primitives

No overlay, no roving-focus engine. Parts that are mostly markup + a little state.

| Component | Source | Gaps vs ShadRz / APG |
| --- | --- | --- |
| **BaseVisuallyHidden** (`BaseVisuallyHiddenRoot`) | **New** | Clip/position CSS in Core wwwroot (ShadRz has no `sr-only` in `theme.css`). |
| **BaseSeparator** (`BaseSeparatorRoot`) | **Port from:** `ShadRz/Base/Separator.cs` | Keep `role="separator"` + `aria-orientation`; drop Tailwind. Optional labeled mode stays unstyled. |
| **BaseLabel** (`BaseLabelRoot`) | **New** (partial overlap with `ShadRz/Base/Field.cs` `FieldLabel`) | Native `<label>` + `For`; no layout classes. |
| **BaseAspectRatio** (`BaseAspectRatioRoot`) | **Port from:** `ShadRz/Base/AspectRatio.cs` | Parse `w:h` → inline `aspect-ratio`; strip `relative` / `absolute inset-0` Tailwind (keep structural inline styles if needed). |
| **BaseButton** (`BaseButtonRoot`) | **Port from:** `ShadRz/Base/Button.cs` | Keep `ElementType`/`Type`/`IsDisabled`/`IsLoading`, Space/Enter on non-`<button>`, `aria-disabled`/`aria-busy`. Drop size/variant/color Tailwind and icon slot styling. |
| **BaseToggle** (`BaseToggleRoot`) | **New** | `role="button"` + `aria-pressed`; `Pressed` / `PressedChanged` / `DefaultPressed`. |
| **BaseAlert** (`BaseAlertRoot`, Title, Description) | **Port from:** `ShadRz/Base/Alert.cs` | Keep `role="alert"`. Add Title/Description parts (ShadRz is a single component). Drop `ButtonColor` theming. |
| **BaseAvatar** (`BaseAvatarRoot`, Image, Fallback) | **Port from:** `ShadRz/Base/Avatar.cs` | Split image into `BaseAvatarImage` (ShadRz owns `<img>` on the root). Load-error → Fallback; `aria-hidden` when image visible. |
| **BaseImage** (`BaseImageRoot`) | **New** | Loading/error slots without Avatar's fallback cascade. |
| **BaseProgress** (`BaseProgressRoot`, Indicator) | **New** | `role="progressbar"`, `aria-valuemin/max/now`; determinate/indeterminate via `data-state`. |
| **BaseTable** (Root, Header, Body, Footer, Row, Head, Cell, Caption) | **Port from:** `ShadRz/Base/Table.cs` | Semantic tags only; drop `TableStyles` Tailwind. No sort/select in v1. |
| **BaseBreadcrumb** (Root, List, Item, Link, Page, Separator) | **New** | `nav` + `aria-label`; current page `aria-current="page"`. |
| **BasePagination** (Root, List, Item, Link, Prev, Next, Ellipsis) | **Port from:** `ShadRz/Base/Pagination.cs` | Port sibling/ellipsis algorithm. Unstyled parts instead of ShadRz `Button` + SVG. |

- [x] BaseVisuallyHidden
- [x] BaseSeparator
- [x] BaseLabel
- [x] BaseAspectRatio
- [x] BaseButton
- [x] BaseToggle
- [x] BaseAlert
- [x] BaseAvatar
- [x] BaseImage
- [x] BaseProgress
- [x] BaseTable
- [x] BaseBreadcrumb
- [x] BasePagination

### Tests (write first)

Default red suite for each P1 component, plus:

- [x] `VisuallyHidden_IsNotVisibleToAT_ButRemainsInA11yTree` (`aria`/clip, not `display:none` without an accessible name strategy).
- [x] `Separator_RoleAndOrientation`; decorative vs labeled.
- [x] `Label_For_AssociatesControl`.
- [x] `AspectRatio_ParsesRatio_SetsInlineAspectRatio`; invalid ratio does not throw unhandled.
- [x] `Button_NonButtonElement_ActivatesOnEnterAndSpace`; `aria-disabled`/`aria-busy`; disabled skips `OnClick`.
- [x] `Toggle_AriaPressed_ControlledAndDefaultPressed`.
- [x] `Alert_RoleAlert_TitleAndDescriptionParts`.
- [x] `Avatar_ImageError_ShowsFallback_HidesFallbackWhenLoaded`; `BaseAvatarImage` is a separate part.
- [x] `Image_LoadingAndErrorSlots`.
- [x] `Progress_RoleProgressbar_ValuenowMinMax_IndeterminateOmitsValuenow`.
- [x] `Table_RendersSemanticTags_CaptionTheadTbodyTfoot`.
- [x] `Breadcrumb_NavLabel_CurrentPageAriaCurrent`.
- [x] `Pagination_SiblingEllipsisAlgorithm` (page 1, middle, last; `SiblingCount`).

### Checks

- [x] `dotnet test` green; every P1 part in [components.md](components.md) has a `Parts_Render` test.
- [x] `CallerClass_Only` fails the build if any P1 component emits a hardcoded utility class.
- [x] Sample has a page per P1 component.
- [x] No `ButtonColor` / size / variant tokens in Core.

---

## P2 — Disclosure and roving focus

Internal `RovingFocusGroup` (tabindex, arrow keys by orientation, Home/End, optional typeahead hook for later phases).

| Component | Source | Gaps vs ShadRz / APG |
| --- | --- | --- |
| **BaseCollapsible** (Root, Trigger, Content) | **Port from:** `ShadRz/Base/Collapsible.cs` | Keep ids, `aria-expanded`/`aria-controls`, `role="region"`, `inert` when closed, `data-state`. Drop grid-rows animation classes. Add `DefaultOpen`. |
| **BaseAccordion** (Root, Item, Header, Trigger, Content) | **Port from:** `ShadRz/Base/Accordion.cs` | Keep single/multiple, collapsible, item keyboard (Enter/Space, arrows, Home/End). Add `aria-disabled` (not only `disabled`), `DefaultValue(s)`, configurable heading via `As`. Drop chevron/border Tailwind. |
| **BaseTabs** (Root, List, Trigger, Content) | **Port from:** `ShadRz/Base/Tabs.cs` | Keep tablist/tab/tabpanel wiring and automatic activation. Add `DefaultValue`. Drop pill/shadow classes. |
| **BaseToggleGroup** (Root, Item) | **New** (builds on BaseToggle + roving focus) | Single/multiple; `aria-pressed`; exclusive mode unpresses siblings. |
| **BaseTreeView** (Root, Item, Trigger, Content) | **New** | APG tree: arrows, expand/collapse, `aria-expanded`/`aria-selected`, nested items. |
| **BaseRadioGroup** (Root, Item, Indicator) | **Port from:** `ShadRz/Base/Radio.cs` | **Do not** wrap native radios. Button-role `radiogroup`/`radio`, roving tabindex, `aria-checked`. Optional hidden input for forms. |
| **BaseCheckbox** (Root, Indicator) | **Port from:** `ShadRz/Base/Checkbox.cs` | Custom `role="checkbox"`, checked/unchecked/**indeterminate**, Space. Drop peer/SVG Tailwind. |
| **BaseCheckboxGroup** (Root, Label, Description, ErrorMessage) | **Port from:** `ShadRz/Base/Checkbox.cs` `CheckboxGroup` | `role="group"`; collection value; wire description/error ids (Form in P3 will share the same pattern). |
| **BaseSwitch** (Root, Thumb) | **New** | `role="switch"`, `aria-checked`; Space/Enter. |

- [ ] Internal `RovingFocusGroup`
- [ ] BaseCollapsible
- [ ] BaseAccordion
- [ ] BaseTabs
- [ ] BaseToggleGroup
- [ ] BaseTreeView
- [ ] BaseRadioGroup
- [ ] BaseCheckbox
- [ ] BaseCheckboxGroup
- [ ] BaseSwitch

### Tests (write first)

Default red suite for each P2 component, plus:

- [ ] `RovingFocusGroup_TabIndexOnlyActiveIsZero`; Arrow/Home/End by orientation; skips disabled; RTL/vertical.
- [ ] `Collapsible_AriaExpandedControls_InertWhenClosed_DefaultOpen`.
- [ ] `Accordion_Single_ClosesOthers`; `Multiple_AllowsMany`; `Collapsible_CanCloseLast`; arrow focus does not toggle; `aria-disabled` on disabled trigger.
- [ ] `Tabs_TablistTabTabpanel_Ids`; automatic activation on arrows; `aria-selected`/`tabindex`.
- [ ] `ToggleGroup_Single_UnpressesSiblings`; `Multiple_Independent`.
- [ ] `TreeView_ArrowKeys_ExpandCollapse_AriaExpandedSelected`.
- [ ] `RadioGroup_RoleRadiogroup_NoNativeInput`; roving + `aria-checked`; optional hidden input when `Name` set.
- [ ] `Checkbox_Indeterminate_AriaCheckedMixed`; Space cycles or sets per spec chosen in implementation (document in test name).
- [ ] `CheckboxGroup_CollectionValue_DescribedByError`.
- [ ] `Switch_RoleSwitch_AriaChecked`.

### Checks

- [ ] `dotnet test` green; keyboard tests exist for Collapsible, Accordion, Tabs, RadioGroup, TreeView, ToggleGroup.
- [ ] No native `<input type="checkbox|radio">` as the **visible** control (hidden form input allowed).
- [ ] Sample pages for all P2 components; README Quick Start can wait until P8 but Collapsible API is the one README will document.

---

## P3 — Form and fields

Port ShadRz field context; integrate with Blazor `EditContext`.

| Component | Source | Gaps vs ShadRz / APG |
| --- | --- | --- |
| **BaseForm** (Root, Field, Label, Control, Description, Message) | **Port from:** `ShadRz/Base/Field.cs` | Keep id generation and `aria-describedby` for description + error (`role="alert"`). Unregister Description/Message on dispose. `EditContext` + `aria-invalid` / `aria-required`. Drop flex/gap/color classes. |
| **BaseInput** (`BaseInputRoot`) | **Port from:** `ShadRz/Base/Input.cs` | Native input + Field wiring. Drop icon-slot Tailwind (icon can be composed outside). |
| **BaseNumberField** (Root, Group, Input, Increment, Decrement) | **New** | Spinbutton APG: arrows, Home/End, min/max/step, `aria-valuenow`. |
| **BaseSlider** (Root, Track, Range, Thumb) | **New** | Pointer + keyboard; `role="slider"`; horizontal/vertical; JS pointer capture in `BaseRz.JS`. |
| **BaseRating** (Root, Item) | **New** | Radio-like or slider APG; keyboard and hover preview. |
| **BaseTagInput** (Root, Input, Tag, TagRemove) | **New** | Add/remove tags; Backspace on empty input; `aria-label` on remove. |
| **BaseSegmentedInput** (Root, Group, Slot, Separator) | **New** | OTP/PIN slots; auto-advance, paste, Backspace. |

- [ ] BaseForm
- [ ] BaseInput
- [ ] BaseNumberField
- [ ] BaseSlider
- [ ] BaseRating
- [ ] BaseTagInput
- [ ] BaseSegmentedInput

### Tests (write first)

Default red suite for each P3 component, plus:

- [ ] `FormField_DescribedBy_IncludesDescriptionAndMessage`; Message `role="alert"` when invalid.
- [ ] `FormField_UnregistersDescribedBy_OnDispose`.
- [ ] `FormField_EditContext_SetsAriaInvalidAndRequired`.
- [ ] `Input_WiresFieldId_AriaInvalidDescribedBy`.
- [ ] `NumberField_Spinbutton_ArrowsHomeEnd_ClampMinMaxStep_AriaValuenow`.
- [ ] `Slider_RoleSlider_KeyboardByOrientation`; pointer path covered with stubbed JS (invoke recorded).
- [ ] `Rating_KeyboardAndHoverPreview_DoesNotCommitUntilActivate` (if hover preview is supported).
- [ ] `TagInput_AddOnEnter_RemoveOnBackspaceEmpty_RemoveHasAccessibleName`.
- [ ] `SegmentedInput_AutoAdvance_Paste_Backspace`.

### Checks

- [ ] `dotnet test` green; Form dispose test proves no stale `aria-describedby` ids.
- [ ] `EditContext` notify covered for Input and NumberField.
- [ ] Slider JS interop is in `BaseRz.JS` (`_content/BaseRz.JS/…`), not ShadRz paths.
- [ ] Sample pages for all P3 components.

---

## P4 — Overlay engine

Move overlay infrastructure out of ShadRz and split it into composable JS/C# building blocks. ShadRz `OverlayHost` currently bakes Tailwind, imperative alerts, and toast layout into one host — BaseRz must not.

**Port from:** `ShadRz/Base/OverlayService.cs`, `ShadRz/Base/OverlayHost.cs`, `ShadRz/Base/OverlayInterop.cs`, `ShadRz/wwwroot/overlay.js`, `ShadRz/Base/Popover.cs` (`PopoverContext`, `OverlayTriggerBase`, `OverlayContentBase`).

Building blocks:

- [ ] **Portal host** — render overlay content at a document-level host (host registers via `AddBaseRz()`).
- [ ] **Dismissable layer** — Escape closes top; outside click; nested stack (`Close` / `CloseTop` semantics from OverlayService).
- [ ] **Focus scope** — trap Tab (existing `trapFocus`/`releaseFocus`); **return focus to trigger on close** (missing in ShadRz).
- [ ] **Scroll lock** — lock body/scroll when modal is open (new).
- [ ] **Positioner** — grow 4-way `PopoverPlacement` into `Side` + `Align`, offset, and collision flip/shift (today: Bottom/Top/Left/Right only).

| Component | Source | Gaps vs ShadRz / APG |
| --- | --- | --- |
| **BasePopover** (Root, Trigger, Anchor, Portal, Content, Close) | **Port from:** `ShadRz/Base/Popover.cs` | Headless trigger/content; no `PanelClass`. Add Anchor, Portal, Close parts. |
| **BaseTooltip** (Provider, Root, Trigger, Portal, Content) | **New** | Provider delay/skip-delay; hover/focus; Escape; no modal trap. |
| **BaseHoverCard** (Root, Trigger, Portal, Content) | **New** | Open on hover with delay; pointer can move into content. |
| **BaseDialog** (Root, Trigger, Portal, Overlay, Content, Title, Description, Close) | **Port from:** `ShadRz/Base/Dialog.cs` | `role="dialog"` + `aria-modal`. **Wire `aria-labelledby` / `aria-describedby` to Title/Description** (missing). `FocusOnOpen` + return focus. |
| **BaseAlertDialog** (Root, Trigger, Portal, Overlay, Content, Title, Description, Action, Cancel) | **Port from:** `ShadRz/Base/Dialog.cs` `AlertDialog` | `role="alertdialog"`. Keep imperative `Show` API on an unstyled host (no `ButtonColor`). Action/Cancel parts; focus first focusable / Cancel by convention. |
| **BaseSheet** (Root, Trigger, Portal, Overlay, Content, Title, Description, Close) | **Port from:** `ShadRz/Base/Sheet.cs` | Edge-anchored modal; drop `OverlayEdgeLayout` Tailwind. Side via `data-side`. |
| **BaseDrawer** (Root, Trigger, Portal, Handle, Overlay, Backdrop, Content, Title, Description, Close) | **Port from:** `ShadRz/Base/Drawer.cs` | Same overlay stack as Sheet. Handle is interactive (swipe/drag via JS), not a visual pill only. |
| **BaseToast** (Provider, Viewport, Root, Title, Description, Action, Close) | **Port from:** `ShadRz/Base/Toast.cs` + queue in `OverlayService` | Split toast queue out of overlay service. `role="status"` (or `alert`), `aria-live`. Pause on hover. Drop Alert/Button chrome. |

- [ ] Overlay JS + C# building blocks
- [ ] BasePopover
- [ ] BaseTooltip
- [ ] BaseHoverCard
- [ ] BaseDialog
- [ ] BaseAlertDialog
- [ ] BaseSheet
- [ ] BaseDrawer
- [ ] BaseToast

`theme.css` overlay-slide-* keyframes stay in ShadRz. BaseRz may expose `data-state` only; consumers animate.

### Tests (write first)

**C# / bUnit** (stub `IJSRuntime`; assert invoke names where JS is required):

- [ ] `OverlayService_CloseTop_ClosesAlertsThenTopOverlay`.
- [ ] `OverlayService_Close_RemovesNestedAboveTarget`.
- [ ] `DismissableLayer_Escape_ClosesTopOnly`.
- [ ] `DismissableLayer_OutsideClick_ClosesWhenCloseOnBackdrop`.
- [ ] `FocusScope_ReturnsFocusToTrigger_OnClose` (this is a ShadRz gap — test must fail on a naive port).
- [ ] `Dialog_AriaModal_LabelledByTitle_DescribedByDescription`.
- [ ] `AlertDialog_RoleAlertdialog_ImperativeShow_CompletesOnAction`.
- [ ] `Popover_OpenBind_AnchorAndPortalParts`; Content not in trigger tree when portaled.
- [ ] `Tooltip_ProviderDelay_EscapeCloses_NoFocusTrap`.
- [ ] `HoverCard_PointerCanMoveIntoContent_WithoutClose`.
- [ ] `Sheet_DataSide`; `Drawer_Handle_InvokesPointerJs`.
- [ ] `Toast_RoleStatus_AriaLive_PauseOnHover_DoesNotUseAlertButtonChrome`.

**JS** (`overlay.js` / positioner):

- [ ] `position_FlipsWhenOverflow`; `align_StartCenterEnd`.
- [ ] `trapFocus_TabsFromLastToFirst`; `releaseFocus_RemovesHandler`.
- [ ] `subscribeEscape_InvokesDotNet`.

### Checks

- [ ] `dotnet test` green; JS tests green.
- [ ] Overlay host markup has **no** Tailwind class strings (`flex`, `bg-black/50`, `max-w-lg`, etc.).
- [ ] Dialog tests require `aria-labelledby` / `aria-describedby` (not optional).
- [ ] Module import paths are `_content/BaseRz.JS/…`.
- [ ] Sample: at least Dialog, Popover, Toast, Tooltip.

---

## P5 — Menus

**Port from:** `ShadRz/Base/MenuCore.cs` (`MenuSession`, `MenuPanelContext`, `MenubarContext`, `IMenuNavigable`, trigger/content/item/sub bases), plus thin shells `ShadRz/Base/DropdownMenu.cs`, `ContextMenu.cs`, `Menubar.cs`.

Menu engine upgrades vs ShadRz:

- Typeahead (character search).
- `menuitemcheckbox` / `menuitemradio` + item indicator.
- `aria-controls` / id pairing on trigger and content.
- Return focus to trigger.
- `data-state` on trigger/content/items.

| Component | Source | Gaps vs ShadRz / APG |
| --- | --- | --- |
| **BaseDropdownMenu** (full part list in components.md) | **Port from:** `DropdownMenu.cs` + MenuCore | Click toggle; all Group/Label/Item/CheckboxItem/RadioGroup/RadioItem/Indicator/Separator/Sub\* parts. |
| **BaseContextMenu** (same menu parts + pointer trigger) | **Port from:** `ContextMenu.cs` | Pointer-anchor + close-on-scroll from ShadRz. Prefer not forcing a focusable `span` wrapper when the trigger is already an element. |
| **BaseMenubar** (Root, Menu, Trigger, Portal, Content, … SubContent) | **Port from:** `Menubar.cs` | `role="menubar"` + orientation. **Roving tabindex across top-level triggers and `MenubarItem`** (ShadRz bar items are not in the panel engine). |
| **BaseNavigationMenu** (Root, List, Item, Trigger, Content, Link, Indicator, Viewport) | **New** | APG disclosure navigation: hover/focus open, Tab through links, viewport/indicator state. |

- [ ] Shared menu engine
- [ ] BaseDropdownMenu
- [ ] BaseContextMenu
- [ ] BaseMenubar
- [ ] BaseNavigationMenu

### Tests (write first)

Default red suite for each P5 component (all parts from [components.md](components.md)), plus:

- [ ] `Menu_RoleMenu_TriggerAriaExpandedAndControls`.
- [ ] `Menu_Typeahead_MovesHighlight`.
- [ ] `Menu_CheckboxItem_RoleMenuitemcheckbox`; `RadioItem_RoleMenuitemradio`; Indicator reflects checked.
- [ ] `Menu_ArrowKeys_SkipDisabled_OpenSubOnRight_CloseSubOnLeft`.
- [ ] `Menu_ReturnFocusToTrigger_OnClose`.
- [ ] `DropdownMenu_ClickToggle`.
- [ ] `ContextMenu_OpensAtPointer_CloseOnScrollFlag`.
- [ ] `Menubar_RovingTabindexOnTriggersAndBarItems`; orientation arrows open adjacent.
- [ ] `NavigationMenu_TabMovesThroughLinks_ViewportIndicatorState`.

### Checks

- [ ] `dotnet test` green; typeahead + checkbox/radio item tests exist (ShadRz gaps).
- [ ] Every Dropdown/Context/Menubar part in [components.md](components.md) has `Parts_Render`.
- [ ] No `MenuStyles` Tailwind leftovers.
- [ ] Sample pages for all P5 components.

---

## P6 — Listbox and scroll

Depends on P2 (roving/typeahead), P3 (Field), P4 (portal/position).

| Component | Source | Gaps vs ShadRz / APG |
| --- | --- | --- |
| **BaseSelect** (Root, Trigger, Value, Portal, Content, Viewport, Group, GroupLabel, Item, ItemText, ItemIndicator, Separator, ScrollUpButton, ScrollDownButton) | **Port from:** `ShadRz/Base/Select.cs` (API only) | ShadRz is a **native `<select>`**. BaseRz is a custom listbox/combobox-button: open state, portal, typeahead, `aria-expanded`/`aria-activedescendant`. Optional hidden `<select>`/`<input>` for forms. |
| **BaseCombobox** (Root, Input, Trigger, Portal, Content, Viewport, Group, GroupLabel, Item, ItemText, ItemIndicator, Empty) | **New** | Filterable listbox; Empty part; keyboard as APG combobox. Share listbox engine with Select. |
| **BaseScrollArea** (Root, Viewport, Scrollbar, Thumb, Corner) | **Port from:** `ShadRz/Base/ScrollArea.cs` | Implement **new** `scrollarea.js` in `BaseRz.JS`. ShadRz imports `./_content/ShadRz/scrollarea.js` but **that file is missing**. `data-state` on scrollbar; native overflow keyboard only. |

- [ ] Shared listbox engine
- [ ] BaseSelect
- [ ] BaseCombobox
- [ ] BaseScrollArea (`scrollarea.js` in `BaseRz.JS`)

### Tests (write first)

- [ ] `Select_IsNotNativeSelect` (no visible `<select>`; listbox/button pattern).
- [ ] `Select_AriaExpanded_ActiveDescendant_Typeahead`.
- [ ] `Select_Groups_Separator_ScrollButtons_ValuePart`.
- [ ] `Select_HiddenInput_WhenNameSet` (form post).
- [ ] `Combobox_FiltersItems_ShowsEmpty_KeyboardApg`.
- [ ] `ScrollArea_Parts_Render`; `data-orientation` / `data-state` on scrollbar.
- [ ] JS: `scrollarea.js` exists and exports attach/dispose (ShadRz file is missing — test the module file is packed as a static web asset).

### Checks

- [ ] `dotnet test` green; `scrollarea.js` is a real static web asset under `_content/BaseRz.JS/`.
- [ ] Select/Combobox share listbox tests for highlight + typeahead (no duplicated bugs).
- [ ] Sample pages for Select, Combobox, ScrollArea.

---

## P7 — Date, time, and color

Depends on P3 (Field/EditContext) and P4 (Popover for pickers). Pointer JS from Slider (P3) reused by ColorPicker.

| Component | Source | Gaps vs ShadRz / APG |
| --- | --- | --- |
| **BaseCalendar** (Root, Header, Title, Nav, Prev/Next, Grid, GridHead/Body, Row, HeadCell, Cell, CellTrigger) | **Port from:** `ShadRz/Base/Calendar.cs` | Port date math, month/year/decade views, culture first-day, min/max/disabled, grid keyboard, `aria-selected`/`current`. **Unstyled parts** instead of ShadRz `Button` + SVG chevrons. Split the monolith into the parts in components.md. |
| **BaseDateField** (Root, Input, Segment) | **New** | Segmented date (year/month/day); arrows increment; Field/`EditContext`. |
| **BaseTimeField** (Root, Input, Segment) | **New** | Same pattern for time segments. |
| **BaseDatePicker** (Root, Trigger, Portal, Content) | **New** | DateField or trigger + Calendar inside Popover. |
| **BaseDateRangePicker** (Root, Trigger, Portal, Content) | **New** | Range selection already sketched in ShadRz Calendar `CalendarRange` — move that into Calendar + this picker. |
| **BaseColorPicker** (Root, Area, Slider, Swatch, Field) | **New** | 2D area + hue/alpha sliders; pointer capture; swatch; text field. |

- [ ] BaseCalendar
- [ ] BaseDateField
- [ ] BaseTimeField
- [ ] BaseDatePicker
- [ ] BaseDateRangePicker
- [ ] BaseColorPicker

### Tests (write first)

- [ ] `Calendar_GridRole_FirstDayOfWeek_Culture`.
- [ ] `Calendar_DisabledDates_MinMax_NotSelectable`.
- [ ] `Calendar_Keyboard_ArrowsPageHomeEndEnter`.
- [ ] `Calendar_Parts_MatchComponentsMd` (no ShadRz `Button` in the tree).
- [ ] `DateField_Segments_ArrowsIncrement_FieldInvalid`.
- [ ] `TimeField_Segments_WrapHours`.
- [ ] `DatePicker_OpensPopover_CommitsCalendarSelection`.
- [ ] `DateRangePicker_SelectsStartThenEnd_HoverPreview`.
- [ ] `ColorPicker_AreaAndSlider_PointerJsRecorded_SwatchAndFieldSync`.

### Checks

- [ ] `dotnet test` green; Calendar has no dependency on ShadRz `Button` / SVG chevron classes.
- [ ] Date/Time fields notify `EditContext`.
- [ ] Sample pages for all P7 components.

---

## P8 — Release

- [ ] NuGet metadata for `BaseRz.Core` and `BaseRz.JS` (authors, license MIT, README, symbols).
- [ ] ARIA/behavior notes under `docs/` (one short page per cluster or per component).
- [ ] WASM sample complete for all [components.md](components.md) entries; GitHub Pages publish.
- [ ] MAUI Blazor Hybrid smoke test (WebView: focus trap, overlays, reset CSS).
- [ ] Align [README.md](../README.md) Quick Start with `BaseCollapsibleRoot` / `@bind-Open` (and `@using BaseRz.Core.Components.Collapsible`).
- [ ] Confirm `AddBaseRz()` + overlay/toast hosts are documented for WASM, Server, and MAUI.

### Tests (write first)

- [ ] `PackagingTests.CoreAndJs_HaveAuthorsLicenseReadme`.
- [ ] `ReadmeTests.QuickStart_UsesBaseCollapsibleRoot_AndBindOpen`.
- [ ] `CatalogTests.EveryComponentsMdRoot_HasSamplePageAndAriaDoc`.
- [ ] `JsModuleTests.NoShadRzContentPaths` (scan Core/JS for `_content/ShadRz`).
- [ ] `HeadlessTests.NoTailwindUtilityLiteralsInCore` (same forbidden prefixes as `CallerClass_Only`).

### Checks

- [ ] `dotnet test` green; `dotnet pack` for Core and JS succeeds.
- [ ] CI (or local script) runs **all** phase Checks: test, pack, forbidden-class scan, `_content/ShadRz` scan.
- [ ] WASM sample builds; GitHub Pages publish path documented.
- [ ] MAUI Hybrid smoke: focus trap, overlay Escape, reset CSS (manual checklist signed off or UI Automation if available).
- [ ] README Quick Start matches [components.md](components.md) naming.

---

## Cross-cutting backlog

Tracked here so phases do not re-litigate them.

- Uncontrolled `Default*` on every stateful root (ShadRz gap).
- Return focus to trigger after modal/menu close.
- Dialog/AlertDialog `aria-labelledby` / `aria-describedby`.
- Menu typeahead, checkbox/radio items, `aria-controls`.
- Custom (non-native) Select, Checkbox, RadioGroup, Switch.
- Missing `scrollarea.js` must be written, not copied.
- No Tailwind, theme tokens, or `ButtonColor`/`ToastColor` in Core.
- JS module URLs use `_content/BaseRz.JS/…`, never `_content/ShadRz/…`.
- TDD: tests in the phase lists are written and failing before production code for that item.

Phase exit: **Tests** for that phase are committed (initially red, then green) and **Checks** are all ticked. Do not start P(n+1) on a red suite from P(n).

## Suggested implementation order (summary)

```
P0 Foundation
 ├─ P1 Static primitives
 ├─ P2 Disclosure + roving focus ──► P3 Form / fields
 └─ P4 Overlay engine
      ├─ P5 Menus
      ├─ P6 Listbox + ScrollArea (also needs P3)
      └─ P7 Date / time / color (also needs P3)
           └─ P8 Release
```

## Coverage

50 roots from [components.md](components.md), each in exactly one phase: P1 (13), P2 (9), P3 (7), P4 (8), P5 (4), P6 (3), P7 (6).

ShadRz headless sources cited above: `Css.cs`, `OverlayService.cs`, `OverlayHost.cs`, `OverlayInterop.cs`, `overlay.js`, `Popover.cs`, `Dialog.cs`, `Sheet.cs`, `Drawer.cs`, `Toast.cs`, `MenuCore.cs`, `DropdownMenu.cs`, `ContextMenu.cs`, `Menubar.cs`, `Field.cs`, `Input.cs`, `Select.cs`, `Checkbox.cs`, `Radio.cs`, `Collapsible.cs`, `Accordion.cs`, `Tabs.cs`, `Button.cs`, `Separator.cs`, `AspectRatio.cs`, `Avatar.cs`, `Alert.cs`, `Table.cs`, `Pagination.cs`, `Calendar.cs`, `ScrollArea.cs`.
