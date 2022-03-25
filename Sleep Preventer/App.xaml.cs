using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using SleepPreventer.Views;

namespace SleepPreventer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string HideParameter = "hide";
        private const string IntervalParameter = "interval";
        private const int DefaultUpdateIntervalInMinutes = 1;
        private const string IntervarRegExp = @"(?<=interval[-=:]*)\d+";

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _ = MessageBox.Show("Sleep Preventer\n\nAn unhandled exception occurred: " + e.Exception.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var startMinimized = false;
            var updateInterval = DefaultUpdateIntervalInMinutes;

            foreach (var arg in e.Args)
            {
                if (arg.Contains(HideParameter))
                {
                    startMinimized = true;
                }
                else if(arg.Contains(IntervalParameter))
                {
                    var match = Regex.Match(arg, IntervarRegExp, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        updateInterval = Convert.ToInt32(match.Value);
                    }
                }
            }

            var mainWindow = new MainWindow(updateInterval);
            Task.Run(() => mainWindow.PreventSleep())
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
}
