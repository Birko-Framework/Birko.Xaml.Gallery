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

## Convention note

Real Avalonia `net8.0` desktop app (`Avalonia.Desktop`) — a dev artifact kept in the framework
workspace. Registered in `Birko.Framework.slnx` (`/Xaml/`) + `.code-workspace`; not in the aggregator.
