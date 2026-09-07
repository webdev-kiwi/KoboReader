using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace KoboReader;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        RestoreWindowSettings();

        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {

        var env = await CoreWebView2Environment.CreateAsync(
            null,
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "KoboReader"
            )
         );

        await Browser.EnsureCoreWebView2Async(env);

        Browser.CoreWebView2.NewWindowRequested += Browser_NewWindowRequested;

        Browser.Source = new Uri("https://www.kobo.com/nz/en/library/books");
    }

    private void Browser_NewWindowRequested(
    object? sender,
    CoreWebView2NewWindowRequestedEventArgs e)
    {
        e.Handled = true;

        Process.Start(new ProcessStartInfo
        {
            FileName = e.Uri,
            UseShellExecute = true
        });
    }

    private readonly string settingsPath =
    Path.Combine(
        Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData),
        "KoboReader",
        "window.json");

    protected override void OnClosing(
    System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);

        Directory.CreateDirectory(
            Path.GetDirectoryName(settingsPath)!);

        var bounds = WindowState == WindowState.Normal
            ? RestoreBounds
            : RestoreBounds;

        var settings = new WindowSettings
        {
            Left = bounds.Left,
            Top = bounds.Top,
            Width = bounds.Width,
            Height = bounds.Height,
            State = WindowState.ToString()
        };

        File.WriteAllText(
            settingsPath,
            JsonSerializer.Serialize(settings));
    }

    private void RestoreWindowSettings()
    {
        if (!File.Exists(settingsPath))
            return;

        var settings =
            JsonSerializer.Deserialize<WindowSettings>(
                File.ReadAllText(settingsPath));

        if (settings == null)
            return;

        Left = settings.Left;
        Top = settings.Top;
        Width = settings.Width;
        Height = settings.Height;

        if (Enum.TryParse(
            settings.State,
            out WindowState state))
        {
            WindowState = state;
        }
    }
}