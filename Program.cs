using Avalonia;

namespace Birko.Xaml.Gallery;

internal static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        // `--screenshots <dir>`: render the gallery headlessly to PNGs and exit (CI, TASK-783). See Screenshots.
        if (args.Length >= 2 && args[0] == "--screenshots") return Screenshots.Run(args[1]);
        return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
