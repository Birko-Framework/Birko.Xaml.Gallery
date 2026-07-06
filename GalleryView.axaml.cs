using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Birko.Xaml.Avalonia.Controls;
using Birko.Xaml.Avalonia.Theming;
using Birko.Xaml.Core.Charts;
using Birko.Xaml.Core.Commands;
using Birko.Xaml.Core.Data;
using Birko.Xaml.Core.Forms;
using Birko.Xaml.Core.Kanban;
using Birko.Xaml.Core.Mvvm;
using Birko.Xaml.Core.Navigation;
using Birko.Xaml.Core.Ribbon;
using Birko.Xaml.Core.Theming;
using Birko.Xaml.Shell.Views;

namespace Birko.Xaml.Gallery;

public sealed class DemoContact
{
    public string? FullName { get; set; } = "Ada Lovelace";
    public string? Email { get; set; } = "ada@example.com";
    public string? Password { get; set; } = "hunter2";
    public bool Subscribed { get; set; } = true;
    public string? Plan { get; set; } = "Pro";
    public double Age { get; set; } = 36;

    public override string ToString() => FullName ?? "(unnamed)";
}

// In-memory data source for the mobile-shell surfaces (the port a real app adapts its store to).
file sealed class GalleryContacts : ICrudDataSource<DemoContact>
{
    private readonly List<DemoContact> _items = new()
    {
        new() { FullName = "Ada Lovelace" }, new() { FullName = "Grace Hopper" }, new() { FullName = "Linus Torvalds" },
    };
    public Task<IReadOnlyList<DemoContact>> GetAllAsync(CancellationToken ct = default) => Task.FromResult<IReadOnlyList<DemoContact>>(_items);
    public Task<DemoContact?> GetAsync(Guid id, CancellationToken ct = default) => Task.FromResult<DemoContact?>(_items.FirstOrDefault());
    public Task<Guid> SaveAsync(DemoContact item, CancellationToken ct = default) { if (!_items.Contains(item)) _items.Add(item); return Task.FromResult(Guid.NewGuid()); }
    public Task DeleteAsync(DemoContact item, CancellationToken ct = default) { _items.Remove(item); return Task.CompletedTask; }
    public DemoContact NewInstance() => new() { FullName = "" };
}

public partial class GalleryView : UserControl
{
    private readonly IThemeManager _themes = new AvaloniaThemeManager();

    public GalleryView()
    {
        AvaloniaXamlLoader.Load(this);
        PopulateForms();
        PopulateViewers();
        PopulateComposites();
        PopulateChrome();
        PopulateOverlays();
        PopulateMobileShell();
    }

    // ── Theme switch ──
    private void OnTheme(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string id })
            _themes.SetTheme(id);
    }

    // ── Ctrl+K opens the palette ──
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.K && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            this.FindControl<CommandPalette>("DemoPalette")!.IsOpen = true;
            e.Handled = true;
        }
    }

    private void PopulateForms()
    {
        var form = this.FindControl<Form>("DemoForm")!;
        form.Fields = new[]
        {
            new FormField { Name = nameof(DemoContact.FullName), Label = "Full name", Required = true, Hint = "As it appears on your ID" },
            new FormField { Name = nameof(DemoContact.Email), Label = "Email", Type = FieldType.Email, Placeholder = "you@example.com" },
            new FormField { Name = nameof(DemoContact.Password), Label = "Password", Type = FieldType.Password, Placeholder = "••••••••" },
            new FormField { Name = nameof(DemoContact.Subscribed), Label = "Subscribed", Type = FieldType.Switch },
            new FormField { Name = nameof(DemoContact.Plan), Label = "Plan", Type = FieldType.Radio,
                Options = new object[] { "Free", "Pro", "Enterprise" } },
            new FormField { Name = nameof(DemoContact.Age), Label = "Age", Type = FieldType.Number, Min = 0, Max = 120, Hint = "0–120" },
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

    private void PopulateViewers()
    {
        this.FindControl<ObjectTree>("DemoJson")!.Json =
            "{\"user\":{\"name\":\"Ada Lovelace\",\"age\":36,\"active\":true},"
            + "\"roles\":[\"admin\",\"editor\"],\"lastLogin\":null}";

        this.FindControl<XmlViewer>("DemoXml")!.Xml =
            "<order id=\"1024\" status=\"open\"><customer>Ada Lovelace</customer>"
            + "<lines><line sku=\"A-1\" qty=\"2\" /><line sku=\"B-9\" qty=\"1\" /></lines></order>";

        this.FindControl<MarkdownEditor>("DemoMarkdown")!.Markdown =
            "# Notes\n\nBirko.Xaml **Tier-2** composites.\n\n- viewers\n- kanban\n- chart\n\n"
            + "Run with `dotnet run`.";
    }

    private void PopulateComposites()
    {
        KanbanColumn Col(string id, string title, params string[] cards)
        {
            var c = new KanbanColumn { Id = id, Title = title };
            foreach (var t in cards) c.Cards.Add(new KanbanCard { Id = t, Title = t });
            return c;
        }
        this.FindControl<Kanban>("DemoKanban")!.Columns = new[]
        {
            Col("todo", "To Do", "Design tokens", "Theme system"),
            Col("doing", "In Progress", "Tier-1 controls"),
            Col("done", "Done", "Gallery", "Core VMs"),
        };

        var chart = this.FindControl<BChart>("DemoChart")!;
        chart.Kind = ChartKind.Line;
        chart.Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
        chart.Series = new[]
        {
            new ChartSeries { Name = "Revenue", Values = new double[] { 12, 18, 15, 24, 30 } },
            new ChartSeries { Name = "Costs", Values = new double[] { 8, 10, 9, 12, 14 } },
        };
    }

    private void PopulateChrome()
    {
        RibbonItem I(string label, string icon) => new() { Id = label, Label = label, Icon = icon };
        this.FindControl<Ribbon>("DemoRibbon")!.Tabs = new[]
        {
            new RibbonTab { Id = "home", Label = "Home", Groups = new[]
            {
                new RibbonGroup { Label = "Clipboard", Items = new[] { I("Paste", "\U0001F4CB"), I("Cut", "✂"), I("Copy", "\U0001F5D0") } },
                new RibbonGroup { Label = "Records", Items = new[] { I("New", "➕"), I("Delete", "\U0001F5D1") } },
            }},
            new RibbonTab { Id = "view", Label = "View", Groups = new[]
            {
                new RibbonGroup { Label = "Zoom", Items = new[] { I("Zoom In", "\U0001F50D"), I("Reset", "↺") } },
            }},
        };
    }

    private void PopulateOverlays()
    {
        this.FindControl<FormModal>("DemoFormModal")!.Fields = new[]
        {
            new FormField { Name = nameof(DemoContact.FullName), Label = "Name", Required = true },
            new FormField { Name = nameof(DemoContact.Email), Label = "Email" },
        };

        this.FindControl<CommandPalette>("DemoPalette")!.Commands = new[]
        {
            new CommandItem { Id = "n", Label = "New contact", Group = "Contacts" },
            new CommandItem { Id = "i", Label = "Import CSV", Group = "Data" },
            new CommandItem { Id = "t", Label = "Toggle theme", Group = "App", Run = () => _themes.SetTheme("dark") },
        };
    }

    private void PopulateMobileShell()
    {
        var data = new GalleryContacts();
        var nav = new NavigationService().Register(
            new ModuleDefinition { Id = "home", Label = "Home", Icon = "🏠", CreateViewModel = () => { var vm = new ListPageViewModel<DemoContact>(data); vm.LoadAsync(); return vm; } },
            new ModuleDefinition { Id = "log", Label = "Log", Icon = "➕", CreateViewModel = () => { var vm = new ListPageViewModel<DemoContact>(data); vm.LoadAsync(); return vm; } },
            new ModuleDefinition { Id = "stats", Label = "Stats", Icon = "📊", CreateViewModel = () => { var vm = new ListPageViewModel<DemoContact>(data); vm.LoadAsync(); return vm; } });
        var shell = new ShellViewModel(nav, _themes) { Title = "Reps" };
        nav.Navigate("home");
        this.FindControl<MobileShellView>("DemoMobileShell")!.DataContext = shell;
    }

    private void OnOpenModal(object? sender, RoutedEventArgs e) =>
        this.FindControl<Modal>("DemoModal")!.IsOpen = true;

    private void OnOpenFormModal(object? sender, RoutedEventArgs e)
    {
        var fm = this.FindControl<FormModal>("DemoFormModal")!;
        fm.Model = new DemoContact { FullName = "", Email = "" };
        fm.IsOpen = true;
    }

    private void OnOpenDrawer(object? sender, RoutedEventArgs e) =>
        this.FindControl<Drawer>("DemoDrawer")!.IsOpen = true;

    private void OnOpenPalette(object? sender, RoutedEventArgs e) =>
        this.FindControl<CommandPalette>("DemoPalette")!.IsOpen = true;
}
