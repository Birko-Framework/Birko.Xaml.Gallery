# Birko.Xaml.Gallery — CLAUDE.md

Runnable Avalonia gallery — the STORY-031 validation gate for EPIC-015. See `README.md` for usage.

## What it is / isn't

- **Is:** a dev/validation artifact — proves the token → ControlTheme → live-swap pipeline visually.
- **Isn't:** a shipped consumer app. No business logic, no packaging.

## Structure

- `App.axaml` — `FluentTheme` base + `BirkoTheme.axaml` (tokens + restyled controls) merged over it.
- `GalleryView` — the demo panel (reused by the headless parity-screenshot test in
  `Birko.Xaml.Avalonia.Tests`). Theme switch buttons call `AvaloniaThemeManager.SetTheme`.
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
