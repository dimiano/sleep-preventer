using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using SleepPreventer.Utils;
using SleepPreventer.ViewModels;

namespace SleepPreventer.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IMainWindow
{
    private readonly MainViewModel _viewModel;
    private Timer _timer;
    private int _timerIntervalSeconds;
    private const int _mouseShift = 5;

    public MainWindow(int updateInterval)
    {
        InitializeComponent();

        _viewModel = new MainViewModel(this);
        _timerIntervalSeconds = updateInterval;
        _viewModel.UpdateInterval = _timerIntervalSeconds;
        _viewModel.SelectedKeyItem = nameof(KeyboardPInvoke.KeyCode.F10);

        MainGrid.DataContext = _viewModel;

        InitTimer(updateInterval);

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
        if (_timerIntervalSeconds == _viewModel.UpdateInterval)
            return;

        _timerIntervalSeconds = _viewModel.UpdateInterval;
        _timer.Stop();
        _timer.Interval = TimeSpan.FromSeconds(_timerIntervalSeconds).TotalMilliseconds;

        if (_viewModel.Enabled)
            _timer.Start();

        _viewModel.UpdateDateTime = DateTime.Now;
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        PreventSleep();
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
}
