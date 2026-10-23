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
    public DateTime? BirthDate { get; set; } = new DateTime(1990, 5, 20);
    public TimeSpan? PreferredTime { get; set; } = new TimeSpan(9, 0, 0);
    public DateTime? Appointment { get; set; }
    public Birko.Xaml.Core.Forms.DateRange? Vacation { get; set; }
    public System.Collections.Generic.List<string> Interests { get; set; } = new() { "Music" };
    public System.Collections.Generic.List<string>? Keywords { get; set; } = new() { "birko", "xaml" };
    public string? Resume { get; set; }

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
            new FormField { Name = nameof(DemoContact.BirthDate), Label = "Birth date", Type = FieldType.Date },
            new FormField { Name = nameof(DemoContact.PreferredTime), Label = "Preferred time", Type = FieldType.Time },
            new FormField { Name = nameof(DemoContact.Appointment), Label = "Appointment", Type = FieldType.DateTime },
            new FormField { Name = nameof(DemoContact.Vacation), Label = "Vacation", Type = FieldType.DateRange },
            new FormField { Name = nameof(DemoContact.Interests), Label = "Interests", Type = FieldType.MultiSelect,
                Options = new object[] { "Music", "Sports", "Travel", "Tech" } },
            new FormField { Name = nameof(DemoContact.Keywords), Label = "Keywords", Type = FieldType.Tags },
            new FormField { Name = nameof(DemoContact.Resume), Label = "Résumé", Type = FieldType.File },
        };
        form.Model = new DemoContact();

        // Clickable crumbs (BreadcrumbItem with Run/Href) — the last is the current location.
        var crumbStatus = this.FindControl<TextBlock>("DemoCrumbStatus")!;
        var crumb = this.FindControl<Breadcrumb>("DemoCrumb")!;
        crumb.ItemsSource = new[]
        {
            new BreadcrumbItem { Label = "Home", Href = "#/", Run = () => crumbStatus.Text = "Navigated: Home" },
            new BreadcrumbItem { Label = "Contacts", Href = "#/contacts", Run = () => crumbStatus.Text = "Navigated: Contacts" },
            new BreadcrumbItem { Label = "Ada Lovelace" }, // current — static
        };
        crumb.ItemInvoked += (_, item) => crumbStatus.Text = $"Navigated: {item.Label} ({item.Href})";

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
        // Every demo command reports itself, so "it ran and the ribbon re-collapsed" is observable. Without
        // this the reviewer could see the reveal close but not whether the command had actually fired.
        RibbonItem I(string label, string icon, string? description = null, string? shortcut = null) => new()
        {
            Id = label, Label = label, Icon = icon, Description = description, Shortcut = shortcut,
            Run = () => this.FindControl<TextBlock>("DemoRibbonLastCommand")!.Text =
                $"{label}  (at {DateTime.Now:HH:mm:ss})",
        };

        static RibbonItem Toggle(RibbonItem i, bool pressed = false, string? set = null) => new()
        {
            Id = i.Id, Label = i.Label, Icon = i.Icon, Description = i.Description, Shortcut = i.Shortcut, Run = i.Run,
            Toggle = set is null, ToggleGroup = set, Pressed = pressed,
        };

        RibbonMenuItem Choice(string label, string? icon = null, string? shortcut = null, bool disabled = false) => new()
        {
            Id = label, Label = label, Icon = icon, Shortcut = shortcut, Disabled = disabled,
            Run = () => this.FindControl<TextBlock>("DemoRibbonLastCommand")!.Text = $"{label}  (at {DateTime.Now:HH:mm:ss})",
        };
        static RibbonItem WithMenu(RibbonItem i, bool split, params RibbonMenuEntry[] menu) => new()
        {
            Id = i.Id, Label = i.Label, Icon = i.Icon, Description = i.Description, Shortcut = i.Shortcut, Run = i.Run,
            Menu = menu, Split = split,
        };

        // Deliberately dense — an Office-sized ribbon (8 tabs, and 6 groups on the active tab), so the
        // overflow behaviour is actually observable: everything fits on a maximized window, and
        // narrowing it reveals the tab-strip and groups-row chevrons (STORY-049/TASK-097). With the
        // old 2-tab / 2-group demo nothing ever overflowed, so the fix could not be reviewed by hand.
        // The launcher reports itself on the same line as the commands (TASK-756).
        this.FindControl<Ribbon>("DemoRibbon")!.GroupLauncherInvoked += (_, e) =>
            this.FindControl<TextBlock>("DemoRibbonLastCommand")!.Text = $"{e.GroupId} dialog  (at {DateTime.Now:HH:mm:ss})";
        // The Quick Access Toolbar (TASK-759): right-click a command to add it. The app saves the choice (the
        // ribbon writes no file); this demo keeps it under LocalApplicationData and restores it at start.
        var demoRibbon = this.FindControl<Ribbon>("DemoRibbon")!;
        demoRibbon.SetQuickAccess(new[] { I("Save", "💾", "Save the document.", "Ctrl+S"), I("Undo", "↶", null, "Ctrl+Z"), I("Redo", "↷", null, "Ctrl+Y") });
        demoRibbon.QuickAccessChanged += (_, refs) =>
        {
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(QuickAccessFile)!);
            System.IO.File.WriteAllLines(QuickAccessFile, refs);
            this.FindControl<TextBlock>("DemoRibbonLastCommand")!.Text = $"Toolbar saved: {string.Join(", ", refs)}";
        };
        this.FindControl<Ribbon>("DemoRibbon")!.Tabs = new[]
        {
            new RibbonTab { Id = "home", Label = "Home", Groups = new[]
            {
                // Priorities are set deliberately so the degrade ORDER is visible while resizing: a lower
                // ScalingPriority gives way first, so Export collapses long before Clipboard is touched.
                // Clipboard is also floored at Small, so it never folds into a flyout however narrow the
                // window gets — the hero group stays directly clickable.
                new RibbonGroup { Label = "Clipboard", Icon = "📋", ScalingPriority = 100, MinSize = RibbonGroupSize.Small,
                    // Descriptions and shortcuts feed the rich tooltips (TASK-753), as in the web Playground's demo.
                    Items = new[]
                    {
                        // A split button: Paste pastes, its ▾ opens the menu (TASK-755), as in the web Playground.
                        WithMenu(I("Paste", "📋", "Add content from the clipboard.", "Ctrl+V"), split: true,
                            Choice("Paste as text", "📝", "Ctrl+Shift+V"), Choice("Paste as picture", "🖼"),
                            RibbonMenuEntry.Separator, Choice("Paste special…", null, "Ctrl+Alt+V")),
                        I("Cut", "✂", "Remove the selection and put it on the clipboard.", "Ctrl+X"),
                        I("Copy", "🗐", "Put a copy of the selection on the clipboard.", "Ctrl+C"),
                    } },
                new RibbonGroup { Label = "Records", Icon = "🗃", ScalingPriority = 80,
                    Items = new[] { I("New", "➕"), I("Delete", "🗑"), I("Duplicate", "⧉") } },
                new RibbonGroup { Label = "Font", Icon = "𝖠", ScalingPriority = 60, Launcher = true, // the corner arrow (TASK-756)
                    // Bold and Italic are toggles, Columns / Rows a one-of set (TASK-754), as in the web Playground.
                    Items = new[]
                    {
                        // Inputs (TASK-762), as in the web Playground: both dropdowns take a value not in their list.
                        new RibbonItem { Id = "font-name", Label = "Font", Description = "Pick or type a font.", Field = new RibbonField
                        {
                            Type = RibbonFieldType.Select, FreeText = true, Value = "Calibri", Width = 10,
                            Options = new[] { "Calibri", "Arial", "Cambria", "Consolas", "Segoe UI" }.Select(f => new RibbonFieldOption { Value = f, Label = f }).ToList(),
                        } },
                        new RibbonItem { Id = "font-size", Label = "Size", Description = "Pick or type a size.", Field = new RibbonField
                        {
                            Type = RibbonFieldType.Select, FreeText = true, Value = "11", Width = 3,
                            Options = new[] { "8", "9", "10", "11", "12", "14", "16", "20", "24" }.Select(v => new RibbonFieldOption { Value = v, Label = v }).ToList(),
                        } },
                        Toggle(I("Bold", "𝐁", "Make the selected text bold.", "Ctrl+B"), pressed: true),
                        Toggle(I("Italic", "𝑰")),
                        I("Colour", "🎨"),
                    } },
                // A gallery (TASK-761), as in the web Playground: framed by default, the look picked under the ribbon.
                new RibbonGroup { Label = "Styles", Icon = "🅰", ScalingPriority = 50,
                    Items = new[]
                    {
                        new RibbonItem { Id = "styles", Label = "Styles", Gallery = new RibbonGallery
                        {
                            Selected = "normal",
                            Items = new[] { ("normal", "Normal", "Aa"), ("h1", "Heading 1", "H1"), ("h2", "Heading 2", "H2"), ("title", "Title", "T"),
                                ("subtitle", "Subtitle", "St"), ("quote", "Quote", "❝"), ("code", "Code", "{}"), ("emphasis", "Emphasis", "𝑒") }
                                .Select(s => new RibbonGalleryItem { Id = s.Item1, Label = s.Item2, Preview = s.Item3 }).ToList(),
                        } },
                    } },
                new RibbonGroup { Label = "Layout", Icon = "▥", ScalingPriority = 40,
                    Items = new[]
                    {
                        Toggle(I("Columns", "▥"), pressed: true, set: "orient"), Toggle(I("Rows", "▤"), set: "orient"),
                        new RibbonItem { Id = "height", Label = "Height", Description = "Row height; ↑ ↓ step it.", Field = new RibbonField
                        {
                            Type = RibbonFieldType.Number, Value = "12", Min = 4, Max = 40, Step = 2, Width = 4,
                        } },
                    } },
                new RibbonGroup { Label = "Review", Icon = "💬", ScalingPriority = 20,
                    Items = new[] { I("Comment", "💬"), I("Track", "✓") } },
                new RibbonGroup { Label = "Export", Icon = "📤", ScalingPriority = 0,
                    Items = new[]
                    {
                        I("PDF", "📄"),
                        WithMenu(I("CSV", "📊"), split: false,
                            Choice("Comma separated"), Choice("Semicolon separated"), Choice("UTF-8 with BOM", disabled: true)),
                        I("Print", "🖨"),
                    } },
            }},
            new RibbonTab { Id = "insert", Label = "Insert", Groups = new[]
            {
                new RibbonGroup { Label = "Media", Items = new[] { I("Image", "\U0001F5BC"), I("Table", "▦") } },
            }},
            new RibbonTab { Id = "design", Label = "Design", Groups = new[]
            {
                new RibbonGroup { Label = "Themes", Items = new[] { I("Palette", "\U0001F3A8"), I("Fonts", "\U0001D5A0") } },
            }},
            new RibbonTab { Id = "data", Label = "Data", Groups = new[]
            {
                new RibbonGroup { Label = "Query", Items = new[] { I("Filter", "\U0001F50D"), I("Sort", "↕") } },
            }},
            new RibbonTab { Id = "view", Label = "View", Groups = new[]
            {
                new RibbonGroup { Label = "Zoom", Items = new[] { I("Zoom In", "\U0001F50D"), I("Reset", "↺") } },
            }},
            new RibbonTab { Id = "automate", Label = "Automate", Groups = new[]
            {
                new RibbonGroup { Label = "Macros", Items = new[] { I("Record", "⏺"), I("Run", "▶") } },
            }},
            new RibbonTab { Id = "developer", Label = "Developer", Groups = new[]
            {
                new RibbonGroup { Label = "Tools", Items = new[] { I("Console", "\U0001F5A5"), I("Inspect", "\U0001F50E") } },
            }},
            new RibbonTab { Id = "help", Label = "Help", Groups = new[]
            {
                new RibbonGroup { Label = "Support", Items = new[] { I("Docs", "\U0001F4D8"), I("Contact", "✉") } },
            }},
            // Contextual tabs (TASK-760): shown while "a table is selected", as in the web Playground.
            new RibbonTab { Id = "table-design", Label = "Table Design", ContextualSet = "table", Groups = new[]
            {
                new RibbonGroup { Label = "Table Styles", Items = new[] { I("Banded Rows", "▤"), I("Header Row", "▔") } },
            }},
            new RibbonTab { Id = "table-layout", Label = "Layout", ContextualSet = "table", Groups = new[]
            {
                new RibbonGroup { Label = "Rows & Columns", Items = new[] { I("Insert Above", "⤒"), I("Insert Below", "⤓"), I("Delete Row", "🗑") } },
            }},
        };
        demoRibbon.FieldChanged += (_, e) =>
            this.FindControl<TextBlock>("DemoRibbonLastCommand")!.Text = $"{e.ItemId} = {e.Value}  (at {DateTime.Now:HH:mm:ss})";
        demoRibbon.GalleryItemPicked += (_, e) =>
            this.FindControl<TextBlock>("DemoRibbonLastCommand")!.Text = $"{e.ParentId}: {e.ItemId}  (at {DateTime.Now:HH:mm:ss})";
        demoRibbon.SetContextualSets(new[] { new RibbonContextualSet { Id = "table", Label = "Table Tools", Color = RibbonContextualColor.Success } });
        if (System.IO.File.Exists(QuickAccessFile)) demoRibbon.SetQuickAccessRefs(System.IO.File.ReadAllLines(QuickAccessFile));
    }

    private static readonly string QuickAccessFile = System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Birko.Xaml.Gallery", "quick-access.txt");

    // TASK-760 — selecting a table shows its contextual set and opens it; deselecting hides it and goes back.
    private void OnRibbonTableSelected(object? sender, RoutedEventArgs e)
    {
        var ribbon = this.FindControl<Ribbon>("DemoRibbon")!;
        if (this.FindControl<CheckBox>("DemoRibbonTableSelected")!.IsChecked == true) ribbon.ShowContextual("table", select: true);
        else ribbon.HideContextual("table");
    }

    private void OnRibbonContextualStyle(object? sender, SelectionChangedEventArgs e)
    {
        if (this.FindControl<Ribbon>("DemoRibbon") is not { } ribbon || sender is not ComboBox box) return;
        ribbon.ContextualStyle = (RibbonContextualStyle)System.Math.Max(0, box.SelectedIndex);
    }

    private void OnRibbonFieldCaption(object? sender, SelectionChangedEventArgs e)
    {
        if (this.FindControl<Ribbon>("DemoRibbon") is not { } ribbon || sender is not ComboBox box) return;
        ribbon.FieldCaption = (RibbonFieldCaption)System.Math.Max(0, box.SelectedIndex);
    }

    private void OnRibbonGalleryStyle(object? sender, SelectionChangedEventArgs e)
    {
        if (this.FindControl<Ribbon>("DemoRibbon") is not { } ribbon || sender is not ComboBox box) return;
        ribbon.GalleryStyle = (RibbonGalleryStyle)System.Math.Max(0, box.SelectedIndex);
    }

    // TASK-101 — the pin toggle, so the two collapse modes are comparable by hand: pinned keeps the body
    // in the layout, unpinned reveals it temporarily as an overlay and re-collapses after a command.
    private void OnToggleRibbonPin(object? sender, RoutedEventArgs e)
    {
        var ribbon = this.FindControl<Ribbon>("DemoRibbon")!;
        ribbon.IsPinned = !ribbon.IsPinned;
        if (!ribbon.IsPinned) ribbon.IsCollapsed = true; // unpinned only means anything while minimised

        this.FindControl<Button>("DemoRibbonPinToggle")!.Content =
            ribbon.IsPinned ? "Unpin the ribbon" : "Pin the ribbon";
        this.FindControl<TextBlock>("DemoRibbonPinState")!.Text = ribbon.IsPinned
            ? "pinned — the body sits in the layout"
            : "unpinned — click a tab to reveal it temporarily over the page";
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
