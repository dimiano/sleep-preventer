using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using SleepPreventer.Settings;
using SleepPreventer.Views;

namespace SleepPreventer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string HideParameter = "hide";
    private const string IntervalParameter = "interval";
    private const string IntervarRegExp = @"(?<=interval[-=:]*)\d+";

    private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show("Sleep Preventer\n\nAn unhandled exception occurred: " + e.Exception.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        var settingsService = new SettingsService();
        var settings = settingsService.Get();
        var startMinimized = false;

        foreach (var arg in e.Args)
        {
            if (arg.Contains(HideParameter))
            {
                startMinimized = true;
            }
            else if (arg.Contains(IntervalParameter))
            {
                var match = Regex.Match(arg, IntervarRegExp, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    settings.Interval = Convert.ToInt32(match.Value);
                }
            }
        }

        var mainWindow = new MainWindow(settings, settingsService);
        Task.Run(mainWindow.PreventSleep)
            .ContinueWith((t) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (startMinimized)
                    {
                        mainWindow.Hide();
                        mainWindow.ShowInTaskbar = false;
                        mainWindow.ShowNotification();
                    }
                    else
                    {
                        mainWindow.Show();
                    }
                });
            });
    }
}
