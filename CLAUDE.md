# Birko.Xaml.Gallery — CLAUDE.md

Runnable Avalonia gallery — the STORY-031 validation gate for EPIC-015. See `README.md` for usage.

## What it is / isn't

- **Is:** a dev/validation artifact — proves the token → ControlTheme → live-swap pipeline visually.
- **Isn't:** a shipped consumer app. No business logic. Packaged only as CI's self-contained, unsigned builds.

## Structure

- `App.axaml` — `FluentTheme` base + `BirkoTheme.axaml` (tokens + restyled controls) merged over it.
- `GalleryView` — the header `Ribbon` (`AppRibbon`: Gallery sections, Appearance themes, Help) over one section at a
  time (the `Tag`'d StackPanels under `#Sections`; `ShowSection(id)`). Themes go through `AvaloniaThemeManager.SetTheme`.
- `DemoCard` (`.cs` + `.axaml`) — every control's card: title, `TypeName`, the `About` line, an ⓘ whose tooltip
  (built in code: a ToolTip popup is out of TemplateBinding's reach) gives `Source` and the `WebTwin`. `Classes="wide"`
  takes the whole row. A new demo goes in a card with all of those set.
- `Screenshots.cs` — `--screenshots <dir>`: headless Skia render of every section (light) and Controls / Viewers in the
  other themes. CI (`.github/workflows/build.yml`) runs it and publishes the PNGs and the win-x64 / linux-x64 /
  osx-arm64 builds as the rolling `latest` release, which the wiki and the landing page link to.
- `MainWindow` hosts `GalleryView`; `Program` is the desktop entry point.

## Conventions / gotchas

- **Consumer app in `Birko/Consumers/`** (mirrors `Birko.Web.Playground`), not a framework project:
  references `Birko.Xaml.*` via `ProjectReference` to `../../Framework/...`, is **not** in
  `Birko.Framework.slnx`/`.code-workspace`, and the framework tests don't depend on it. Real Avalonia
  `net8.0` `WinExe` (Avalonia 11.2.3). Run/validate standalone with `dotnet run`.
- `Application.Resources` needs a `ResourceDictionary` + `MergedDictionaries` wrapper around a
  `ResourceInclude` (a bare `ResourceInclude` there fails to compile).
- When adding controls here, prefer ones already restyled in `Birko.Xaml.Avalonia/Controls`. As
  STORY-034 adds Tier-1 controls, extend `GalleryView` to show them (and the screenshot test covers them).
- Parity screenshots are reproducible, not committed: `BIRKO_SHOTS=<dir> dotnet test …Avalonia.Tests`.

- **The header ribbon has `CollapseShortcut` null and `KeyTipKey` None.** Both are window-wide, and the Chrome section's
  demo ribbon is the one demonstrating them; two ribbons answering Ctrl+F1 / Alt would fight (the web Playground hit
  the same, TASK-781).
