using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;
using SleepPreventer.Settings;
using SleepPreventer.Tools;
using SleepPreventer.ViewModels;
using Timer = System.Timers.Timer;

namespace SleepPreventer.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IMainWindow
{
    private static readonly HttpClient HttpClient = new();
    private readonly MainViewModel _viewModel;
    private Timer _timer;
    private int _timerIntervalSeconds;
    private readonly SettingsService _settingsService;
    private const int _mouseShift = 5;
    private string _lastUrlResponse;
    // prevent timer from firing again while the NotifyPopup window is open.
    private volatile bool _isUrlUpdateMessageBoxOpen;
    private bool _isTimerDisposed;
    // Prevent overlapping URL checks. Timer can fire again while the previous async GET is still waiting,
    // because the timer doesn’t wait for the async void event handler to finish.
    // Overlapping requests could complete out of order and cause incorrect comparisons or notifications.
    private readonly SemaphoreSlim _urlPollingLock = new(1, 1);

    internal MainWindow(AppSettings settings, SettingsService settingsService)
    {
        InitializeComponent();

        _settingsService = settingsService;
        _viewModel = new MainViewModel(this);
        _timerIntervalSeconds = settings.Interval;
        _viewModel.UpdateInterval = _timerIntervalSeconds;
        _viewModel.SelectedKeyItem = Enum.GetName(settings.Key);
        _viewModel.Url = settings.Url;

        MainGrid.DataContext = _viewModel;

        InitTimer(settings.Interval);

        SystemEvents.SessionSwitch += OnSessionSwitch;
    }

    private void InitTimer(int updateIntervalSeconds)
    {
        _timer = new Timer();
        _timer.Interval = TimeSpan.FromSeconds(updateIntervalSeconds).TotalMilliseconds;
        _timer.Elapsed += OnTimerElapsed;
        _timer.Start();
    }

    public void TimerDispose()
    {
        _isTimerDisposed = true;
        _timer.Stop();
        _timer.Elapsed -= OnTimerElapsed;
        _timer.Dispose();
    }

    public void ShowNotification()
    {
        //// Hardcodet.NotifyIcon.Wpf package
        //if (_viewModel.IsTrayPopupEnables)
        //{
        //    Hardcodet.Wpf.TaskbarNotification.NotifyIcon.ShowBalloonTip(
        //        "Sleep Preventer is working ...",
        //        $"Status: {_viewModel.Status}, Interval: {_timerIntervalSeconds} sec",
        //        BalloonIcon.Info);
        //}
    }

    public void CloseApp()
    {
        TimerDispose();
        SystemEvents.SessionSwitch -= OnSessionSwitch;

        Application.Current.Shutdown();
    }

    public void PreventSleep()
    {
        if (_viewModel.Enabled)
        {
            // Prevent screen from sleeping
            ScreenSaverPreventer.PreventScreenSaver(_viewModel.Enabled);

            // Prevent teams from put away status
            KeyboardHelper.KeyDoublePress(_viewModel.SelectedKey);
            //MouseHelper.MoveCursorBy(_MouseShift);

            _viewModel.UpdateDateTime = DateTime.Now;
        }
    }

    public void ToggleWindowVisibility(bool isForceShow)
    {
        if (isForceShow)
        {
            ShowInTaskbar = true;
            Visibility = Visibility.Visible;
            WindowState = WindowState.Normal;
        }
        else
        {
            ShowInTaskbar = !ShowInTaskbar;
            if (ShowInTaskbar)
            {
                WindowState = WindowState.Normal;
                Visibility = Visibility.Visible;
            }
            else
            {
                Hide();
                ShowNotification();
            }
            //WindowState = ShowInTaskbar ? WindowState.Normal : WindowState.Minimized;
            //Visibility = ShowInTaskbar ? Visibility.Visible : Visibility.Hidden;
            //ShowNotification();
        }
    }

    public void UpdateSettings()
    {
        if (!AppSettings.IsIntervalValid(_viewModel.UpdateInterval))
        {
            //MessageBox.Show(this,
            //    $"Interval must be greater than 0 and less than {AppSettings.SecondsPerDay} seconds.",
            //    "Invalid interval",
            //    MessageBoxButton.OK,
            //    MessageBoxImage.Warning);
            _viewModel.UpdateInterval = _timerIntervalSeconds;
            return;
        }

        _settingsService.Save(new AppSettings
        {
            Interval = _viewModel.UpdateInterval,
            Key = _viewModel.SelectedKey,
            Url = _viewModel.Url
        });

        if (_timerIntervalSeconds == _viewModel.UpdateInterval)
            return;

        _timerIntervalSeconds = _viewModel.UpdateInterval;
        _timer.Stop();
        _timer.Interval = TimeSpan.FromSeconds(_timerIntervalSeconds).TotalMilliseconds;

        if (_viewModel.Enabled && !_isUrlUpdateMessageBoxOpen)
            _timer.Start();

        _viewModel.UpdateDateTime = DateTime.Now;
    }

    private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        if (_isUrlUpdateMessageBoxOpen)
            return;

        PreventSleep();
        await CheckUrlAsync();
    }

    private async Task CheckUrlAsync()
    {
        if (_isUrlUpdateMessageBoxOpen ||
            string.IsNullOrWhiteSpace(_viewModel.Url) ||
            !await _urlPollingLock.WaitAsync(0))
            return;

        try
        {
            var messageTitle = "Sleep Preventer";
            var messageText = "URL data updated!";

            var response = await HttpClient.GetStringAsync(_viewModel.Url);

            if (_lastUrlResponse is not null &&
                !string.Equals(_lastUrlResponse, response, StringComparison.Ordinal))
            {
                Dispatcher.Invoke(() =>
                {
                    var restartTimer = _timer.Enabled;
                    _isUrlUpdateMessageBoxOpen = true;
                    _timer.Stop();
                    try
                    {
                        NotifyIcon.ShowBalloonTip(messageTitle, messageText, BalloonIcon.Info);
                        MessageBox.Show(messageText, messageTitle, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                    }
                    finally
                    {
                        _isUrlUpdateMessageBoxOpen = false;
                        if (restartTimer && !_isTimerDisposed)
                            _timer.Start();
                    }
                });
            }
            _viewModel.DataLength = response.Length;
            _lastUrlResponse = response;
        }
        catch (HttpRequestException)
        {
            return;
        }
        catch (TaskCanceledException)
        {
            return;
        }
        catch (UriFormatException)
        {
            return;
        }
        finally
        {
            _urlPollingLock.Release();
        }
    }

    private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        if (e.Reason is SessionSwitchReason.SessionUnlock or SessionSwitchReason.SessionLogon)
        {
            PreventSleep();
        }
    }

    private void EnabledStateChanged(object sender, RoutedEventArgs e)
    {
        _viewModel.Status = _viewModel.Enabled ? "Enabled" : "Disabled";
        btnEnabledToggle.Background = _viewModel.Enabled ? Brushes.Green : Brushes.Gray;
    }

    private void SelectedKeyChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded)
            UpdateSettings();
    }
}
