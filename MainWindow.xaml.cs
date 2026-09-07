using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace KoboReader;

public partial class MainWindow : Window
{
    private WindowState _previousWindowState;
    private WindowStyle _previousWindowStyle;
    private ResizeMode _previousResizeMode;
    private Rect _previousBounds;
    private bool _previousTopmost;
    private bool _isFullscreen = false;
    private bool _startInFullscreen;

    public MainWindow()
    {
        InitializeComponent();

        RestoreWindowSettings();

        Loaded += MainWindow_Loaded;

        KeyDown += MainWindow_KeyDown;
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

        if (_startInFullscreen)
        {
            ToggleFullscreen();
        }
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

        var bounds = RestoreBounds;

        var settings = new WindowSettings
        {
            Left = bounds.Left,
            Top = bounds.Top,
            Width = bounds.Width,
            Height = bounds.Height,
            State = WindowState.ToString(),
            Fullscreen = _isFullscreen
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

        bool validPosition = false;

        foreach (var screen in Forms.Screen.AllScreens)
        {
            if (settings.Left >= screen.Bounds.Left &&
                settings.Left < screen.Bounds.Right &&
                settings.Top >= screen.Bounds.Top &&
                settings.Top < screen.Bounds.Bottom)
            {
                validPosition = true;
                break;
            }
        }

        if (validPosition)
        {
            Left = settings.Left;
            Top = settings.Top;
            Width = settings.Width;
            Height = settings.Height;
        }
        else
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        if (Enum.TryParse(
            settings.State,
            out WindowState state))
        {
            WindowState = state;
        }

        _startInFullscreen = settings.Fullscreen;
    }

    private void MainWindow_KeyDown(
    object sender,
    System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.F11)
        {
            ToggleFullscreen();
        }

        if (Keyboard.Modifiers ==
                (ModifierKeys.Control | ModifierKeys.Shift)
            && e.Key == Key.F)
        {
            ToggleFullscreen();
        }

        if (e.Key == System.Windows.Input.Key.Escape &&
            _isFullscreen)
        {
            ToggleFullscreen();
        }
    }

    private void ToggleFullscreen()
    {
        if (!_isFullscreen)
        {
            _previousWindowState = WindowState;
            _previousWindowStyle = WindowStyle;
            _previousResizeMode = ResizeMode;
            _previousTopmost = Topmost;
            _previousBounds = RestoreBounds;

            WindowState = WindowState.Normal;

            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Topmost = true;

            var helper = new WindowInteropHelper(this);

            var screen = Forms.Screen.FromHandle(helper.Handle);

            Left = screen.Bounds.Left;
            Top = screen.Bounds.Top;
            Width = screen.Bounds.Width;
            Height = screen.Bounds.Height;

            _isFullscreen = true;
        }
        else
        {
            Topmost = _previousTopmost;

            WindowStyle = _previousWindowStyle;
            ResizeMode = _previousResizeMode;
            WindowState = WindowState.Normal;

            Left = _previousBounds.Left;
            Top = _previousBounds.Top;
            Width = _previousBounds.Width;
            Height = _previousBounds.Height;

            WindowState = _previousWindowState;

            _isFullscreen = false;
        }
    }
}