using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace KoboReader;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

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
}