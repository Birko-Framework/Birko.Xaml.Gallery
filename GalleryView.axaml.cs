using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Birko.Xaml.Avalonia.Controls;
using Birko.Xaml.Avalonia.Theming;
using Birko.Xaml.Core.Forms;
using Birko.Xaml.Core.Theming;

namespace Birko.Xaml.Gallery;

public sealed class DemoContact
{
    public string? FullName { get; set; } = "Ada Lovelace";
    public string? Email { get; set; } = "ada@example.com";
    public bool Subscribed { get; set; } = true;
    public string? Plan { get; set; } = "Pro";
}

public partial class GalleryView : UserControl
{
    private readonly IThemeManager _themes = new AvaloniaThemeManager();

    public GalleryView()
    {
        AvaloniaXamlLoader.Load(this);

        var form = this.FindControl<Form>("DemoForm")!;
        form.Fields = new[]
        {
            new FormField { Name = nameof(DemoContact.FullName), Label = "Full name", Required = true },
            new FormField { Name = nameof(DemoContact.Email), Label = "Email", Placeholder = "you@example.com" },
            new FormField { Name = nameof(DemoContact.Subscribed), Label = "Subscribed", Type = FieldType.Checkbox },
            new FormField { Name = nameof(DemoContact.Plan), Label = "Plan", Type = FieldType.Select,
                Options = new object[] { "Free", "Pro", "Enterprise" } },
        };
        form.Model = new DemoContact();

        this.FindControl<Breadcrumb>("DemoCrumb")!.ItemsSource = new[] { "Home", "Contacts", "Ada Lovelace" };

        this.FindControl<DataGrid>("DemoGrid")!.ItemsSource = new[]
        {
            new DemoContact { FullName = "Ada Lovelace", Email = "ada@example.com", Plan = "Pro" },
            new DemoContact { FullName = "Grace Hopper", Email = "grace@example.com", Plan = "Enterprise" },
            new DemoContact { FullName = "Linus Torvalds", Email = "linus@example.com", Plan = "Free" },
        };
    }

    private void OnTheme(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string id })
            _themes.SetTheme(id);
    }
}
