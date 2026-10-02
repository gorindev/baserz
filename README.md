# BaseRz

An unstyled, accessible, headless UI component library built for **Blazor** (WebAssembly, Server) and **.NET MAUI Blazor Hybrid** applications.

`BaseRz` provides fully accessible, keyboard-navigable UI primitives and state management logic without dictating CSS or layout styles. It serves as the foundation for design systems and UI libraries like `ShadRz`.

---

## Features

- **Headless Architecture:** Zero opinionated CSS. Complete design freedom over markup and styles.
- **WAI-ARIA Compliant:** Implements standard ARIA roles, states, and keyboard navigation behaviors out of the box.
- **Cross-Platform:** Runs seamlessly on Blazor WebAssembly, Blazor Server, and .NET MAUI Blazor Hybrid apps.
- **Lightweight JSInterop:** Minimal JavaScript footprints isolated into platform-agnostic adapters for focus trapping, popovers, and DOM listeners.
- **Opt-In Reset:** Optional, ultra-lightweight CSS reset (`baserz-reset.css`) to normalize cross-browser and WebView rendering.

---

## Installation

Install the core headless library via NuGet:

```bash
dotnet add package BaseRz.Core
```

Register the services in `Program.cs` (WASM, Server, or `MauiProgram.cs`):

```csharp
builder.Services.AddBaseRz();
```

---

## Quick Start

BaseRz uses compound components to expose structured control over rendering while managing underlying state automatically.

```razor
@using BaseRz.Core.Components.Collapsible

<CollapsibleRoot @bind-IsOpen="_isOpen">
    <CollapsibleTrigger class="my-trigger-button">
        Toggle Content
    </CollapsibleTrigger>
    
    <CollapsibleContent class="my-content-panel">
        <p>This content expands and collapses with full ARIA accessibility support.</p>
    </CollapsibleContent>
</CollapsibleRoot>

@code {
    private bool _isOpen = false;
}
```

---

## Repository Architecture

```plaintext
Plaintext
BaseRz/
├── docs/                     # Conceptual documentation & ARIA specifications
├── samples/
│   └── BaseRz.Samples.Wasm/  # Blazor WASM showcase app (GitHub Pages)
├── src/
│   ├── BaseRz.Core/          # Headless Razor components & C# state primitives
│   └── BaseRz.JS/            # Isolated JSInterop modules for DOM & A11y management
└── tests/
    └── BaseRz.Tests/         # bUnit unit and integration test suite
```

---

## Optional CSS Reset

If you want a neutral baseline for cross-platform apps (such as alignment between Web and .NET MAUI WebViews), import the optional reset in your index.html or App.razor:

```html
<link href="_content/BaseRz.Core/css/baserz-reset.css" rel="stylesheet" />
```

---

## License

Distributed under the MIT License. See `LICENSE` for details.
