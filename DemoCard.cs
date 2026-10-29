using Avalonia;
using Avalonia.Controls;

namespace Birko.Xaml.Gallery;

/// <summary>
/// One gallery card: the control's name and type, a line saying what it is, an ⓘ whose tooltip says where it comes
/// from and what it pairs with on the web, and the live demo as content. The same shape as a Birko.Web.Playground card
/// (TASK-783), so the two galleries read alike.
/// </summary>
public class DemoCard : ContentControl
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<DemoCard, string?>(nameof(Title));

    /// <summary>The type a consumer writes in AXAML, e.g. <c>c:JsonDiffViewer</c> or <c>DataGrid</c>.</summary>
    public static readonly StyledProperty<string?> TypeNameProperty =
        AvaloniaProperty.Register<DemoCard, string?>(nameof(TypeName));

    /// <summary>One sentence: what it is and what it is for. Shown under the title.</summary>
    public static readonly StyledProperty<string?> AboutProperty =
        AvaloniaProperty.Register<DemoCard, string?>(nameof(About));

    /// <summary>Where it comes from (a Birko control or a restyled Avalonia one) — the ⓘ tooltip's first line.</summary>
    public static readonly StyledProperty<string?> SourceProperty =
        AvaloniaProperty.Register<DemoCard, string?>(nameof(Source));

    /// <summary>The Birko.Web component it matches, e.g. <c>b-json-diff</c>; empty when it has none.</summary>
    public static readonly StyledProperty<string?> WebTwinProperty =
        AvaloniaProperty.Register<DemoCard, string?>(nameof(WebTwin));

    public string? Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string? TypeName { get => GetValue(TypeNameProperty); set => SetValue(TypeNameProperty, value); }
    public string? About { get => GetValue(AboutProperty); set => SetValue(AboutProperty, value); }
    public string? Source { get => GetValue(SourceProperty); set => SetValue(SourceProperty, value); }
    public string? WebTwin { get => GetValue(WebTwinProperty); set => SetValue(WebTwinProperty, value); }

    /// <summary>The words the filter and the tooltip match against.</summary>
    public string SearchText => $"{Title} {TypeName} {About} {WebTwin}".ToLowerInvariant();

    // The ⓘ tooltip is built here rather than in the template: a ToolTip opens in its own popup, where a TemplateBinding
    // does not reach. Its text is also the button's accessible help text, so a screen reader hears what a pointer sees.
    protected override void OnApplyTemplate(global::Avalonia.Controls.Primitives.TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (e.NameScope.Find<Button>("PART_Info") is not { } info) return;

        var lines = new List<(string Key, string Value)>();
        if (!string.IsNullOrEmpty(Source)) lines.Add(("From", Source));
        if (!string.IsNullOrEmpty(TypeName)) lines.Add(("In AXAML", TypeName));
        lines.Add(("On the web", string.IsNullOrEmpty(WebTwin) ? "no counterpart" : WebTwin));

        var tip = new StackPanel { Spacing = 4, MaxWidth = 320 };
        tip.Children.Add(new TextBlock { Text = Title, FontWeight = global::Avalonia.Media.FontWeight.SemiBold });
        if (!string.IsNullOrEmpty(About)) tip.Children.Add(new TextBlock { Text = About, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap });
        foreach (var (key, value) in lines)
            tip.Children.Add(new TextBlock { Text = $"{key}: {value}", TextWrapping = global::Avalonia.Media.TextWrapping.Wrap, Opacity = 0.85 });

        ToolTip.SetTip(info, tip);
        ToolTip.SetShowDelay(info, 200);
        global::Avalonia.Automation.AutomationProperties.SetName(info, $"About {Title}");
        global::Avalonia.Automation.AutomationProperties.SetHelpText(info,
            string.Join(". ", new[] { About ?? "" }.Concat(lines.Select(l => $"{l.Key}: {l.Value}"))));
        // A click shows it too, so it is reachable by touch and keyboard, not only by hover.
        info.Click += (_, _) => ToolTip.SetIsOpen(info, !ToolTip.GetIsOpen(info));
    }
}
