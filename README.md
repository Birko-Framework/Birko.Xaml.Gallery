# Birko.Xaml.Gallery

The Birko design system on the desktop: every Birko.Xaml control, live, in all four themes — the Avalonia counterpart
of the [Birko.Web Playground](https://birko-framework.github.io/Birko.Web.Playground/). It began as the STORY-031
go/no-go gate for EPIC-015 and is still a validation artifact, **not** a shipped consumer.

![The Controls section in the light theme](https://github.com/Birko-Framework/Birko.Xaml.Gallery/releases/latest/download/gallery-light-controls.png)

## Download

Built on every push to `main` and published as the [latest release](https://github.com/Birko-Framework/Birko.Xaml.Gallery/releases/latest) — self-contained, no .NET install needed:

| System | Download |
|---|---|
| Windows x64 | [Birko.Xaml.Gallery-win-x64.zip](https://github.com/Birko-Framework/Birko.Xaml.Gallery/releases/latest/download/Birko.Xaml.Gallery-win-x64.zip) — unzip, run `Birko.Xaml.Gallery.exe` |
| Linux x64 | [Birko.Xaml.Gallery-linux-x64.tar.gz](https://github.com/Birko-Framework/Birko.Xaml.Gallery/releases/latest/download/Birko.Xaml.Gallery-linux-x64.tar.gz) — `tar -xzf`, run `./Birko.Xaml.Gallery/Birko.Xaml.Gallery` |
| macOS (Apple Silicon) | [Birko.Xaml.Gallery-osx-arm64.zip](https://github.com/Birko-Framework/Birko.Xaml.Gallery/releases/latest/download/Birko.Xaml.Gallery-osx-arm64.zip) — unzip, then `xattr -dr com.apple.quarantine Birko.Xaml.Gallery && ./Birko.Xaml.Gallery/Birko.Xaml.Gallery` |

The builds are not code-signed: Windows SmartScreen asks once ("More info → Run anyway"), and macOS needs the
quarantine flag cleared as shown.

## Run it from source

```bash
dotnet run --project Birko.Xaml.Gallery
```

It references `Birko.Xaml.*` at `../../Framework/`, so check it out in the `Birko/Consumers` + `Birko/Framework` layout.

## What it shows

The header is Birko's own `Ribbon` (TASK-783), as on the web Playground:

- **Gallery** picks a section — Controls, Forms & data, Viewers, Composites, Chrome (the ribbon on its own, with its
  looks to try), Overlays, Mobile shell.
- **Appearance** switches the four themes live (Light, Dark, Neon, Finstat), generated from the same `tokens.json` as the
  web CSS.
- **Help** opens About and the links: source, guide, the web gallery, downloads.

Every control sits in a card that says what it is; **ⓘ** says where it comes from (a Birko control, or an Avalonia one
in the Birko look), how it is written in AXAML, and which Birko.Web component it matches.

## Screenshots

`Birko.Xaml.Gallery --screenshots <dir>` renders every section headlessly (Skia, no display) in the light theme, plus
Controls and Viewers in the other three, and exits. CI runs it on every push and attaches the PNGs to the latest
release, so the wiki and https://birko-framework.github.io/ always show the current build:
`https://github.com/Birko-Framework/Birko.Xaml.Gallery/releases/latest/download/gallery-<theme>-<section>.png`.

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
