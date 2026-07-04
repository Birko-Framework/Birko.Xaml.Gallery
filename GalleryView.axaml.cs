using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Birko.Xaml.Avalonia.Theming;
using Birko.Xaml.Core.Theming;

namespace Birko.Xaml.Gallery;

public partial class GalleryView : UserControl
{
    private readonly IThemeManager _themes = new AvaloniaThemeManager();

    public GalleryView() => AvaloniaXamlLoader.Load(this);

    private void OnTheme(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string id })
            _themes.SetTheme(id);
    }
}
