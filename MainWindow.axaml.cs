using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Birko.Xaml.Gallery;

public partial class MainWindow : Window
{
    public MainWindow() => AvaloniaXamlLoader.Load(this);
}
