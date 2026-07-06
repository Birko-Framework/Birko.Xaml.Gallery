# Birko.Xaml.Gallery

A tiny runnable Avalonia desktop app that renders the first Birko restyled controls across all four
themes. It's the **STORY-031 go/no-go gate** for EPIC-015: a dev/validation artifact (mirroring how
the Birko.Web Playground validated components), **not** a shipped consumer.

## Run it

```bash
dotnet run --project Birko.Xaml.Gallery
```

Click **Light / Dark / Neon / Finstat** — the Button, TextBox, Card and Badge re-theme live via
`RequestedThemeVariant` (no restart). Colors, corner radius and typography all come from the design
tokens shared byte-for-byte with Birko.Web.

## What it shows

Organized into `TabControl` sections (which also dogfoods `b-tabs`):
- **Controls** — buttons, badge/tag, input, combobox, checkbox/radio/toggle, progress + spinner, dropdown menu, breadcrumb, tooltip.
- **Forms & data** — schema-driven `Form`, `DataGrid`, `TreeView`.
- **Viewers** — `ObjectTree` (JSON), `XmlViewer`, `MarkdownEditor`.
- **Composites** — `Kanban`, `BChart` (LiveCharts2).
- **Overlays** — buttons that open the `Modal`, `FormModal`, `Drawer`, and the `CommandPalette` (also **Ctrl+K**), hosted at the window root.

## What it validates

- The generated `Tokens.axaml` + restyled `ControlThemes` (`BirkoTheme.axaml`) load and apply.
- Runtime theme swap re-resolves every `{DynamicResource}` live.
- Visual parity with Birko.Web: same token values → same look (incl. finstat's flat/square corners).

## Parity screenshots (automated)

`Birko.Xaml.Avalonia.Tests` renders this view headlessly (Skia) and writes one PNG per theme —
regenerate with:

```bash
BIRKO_SHOTS=./shots dotnet test ../../Framework.Tests/Birko.Xaml.Avalonia.Tests
```

## Verdict (2026-07-04): GO

All four themes render distinctly and correctly; the token pipeline delivers parity and live swap.
Tier-1 (STORY-034, the full restyled-control sweep) is cleared to proceed.

## Where it lives

A **consumer app** in `Birko/Consumers/` (mirroring `Birko.Web.Playground`) — it *consumes* the
`Birko.Xaml.*` framework assemblies via `ProjectReference` across the bucket boundary
(`../../Framework/...`). It is deliberately **not** in `Birko.Framework.slnx` or the framework
`.code-workspace`, and the framework test suite does not depend on it: like the Web playground, the
gallery is a standalone demo/validation surface you run directly (`dotnet run`), not part of the
shipped framework.
