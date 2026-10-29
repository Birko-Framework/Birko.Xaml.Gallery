using Avalonia;
using Avalonia.Headless;
using Avalonia.Threading;

namespace Birko.Xaml.Gallery;

/// <summary>
/// <c>Birko.Xaml.Gallery --screenshots &lt;dir&gt;</c>: renders the gallery headlessly (Skia, no display) and writes one
/// PNG per section in the light theme, plus the Controls and Viewers sections in every other theme (TASK-783). CI runs it
/// on every push and attaches the PNGs to the release, so the wiki and the landing page show the current app, not a
/// screenshot someone took once.
/// </summary>
internal static class Screenshots
{
    public static int Run(string dir)
    {
        Directory.CreateDirectory(dir);
        AppBuilder.Configure<App>()
            .UseSkia()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false })
            .WithInterFont()
            .SetupWithoutStarting();

        var window = new MainWindow { Width = 1280, Height = 820 };
        window.Show();
        var view = (GalleryView)window.Content!;
        var written = 0;

        foreach (var theme in view.Themes.Available)
        {
            view.Themes.SetTheme(theme.Id);
            var sections = theme.Id == "light" ? GalleryView.SectionIds : new[] { "controls", "viewers" };
            foreach (var section in sections)
            {
                view.ShowSection(section);
                // Layout, then a few render ticks: the charts and the diff viewer finish on a later pass.
                for (var i = 0; i < 6; i++)
                {
                    Dispatcher.UIThread.RunJobs();
                    AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                }
                var frame = window.CaptureRenderedFrame();
                if (frame is null) { Console.Error.WriteLine($"no frame for {theme.Id}/{section}"); return 1; }
                var path = Path.Combine(dir, $"gallery-{theme.Id}-{section}.png");
                frame.Save(path);
                Console.WriteLine(path);
                written++;
            }
        }
        window.Close();
        return written > 0 ? 0 : 1;
    }
}
