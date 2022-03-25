using Hardcodet.Wpf.TaskbarNotification;
using SleepPreventer.ViewModels;
using Microsoft.Win32;
using System;
using System.Timers;
using System.Windows;

namespace SleepPreventer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        private readonly MainViewModel _viewModel;
        private Timer _timer;

        public MainWindow(int updateInterval)
        {
            InitializeComponent();

            _viewModel = new MainViewModel(this);
            _viewModel.UpdateInterval = updateInterval;

            MainGrid.DataContext = _viewModel;

            InitTimer(updateInterval);

            SystemEvents.SessionSwitch += OnSessionSwitch;
        }

        private void InitTimer(int updateInterval)
        {
            _timer = new Timer();
            _timer.Interval = TimeSpan.FromMinutes(updateInterval).TotalMilliseconds;
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
            NotifyIcon.ShowBalloonTip(
                "Sleep Preventer is working ...",
                $"Status: {_viewModel.Status}, Interval: {_timer.Interval}",
                BalloonIcon.Info);
        }

        public void PreventSleep()
        {
            _timer.Interval = TimeSpan.FromMinutes(_viewModel.UpdateInterval).TotalMilliseconds;
            _viewModel.Status = _viewModel.IsEnabled ? "Enabled" : "Disabled";

            //var msgResult = MessageBox.Show(
            //    this,
            //    $"Flat count updated: {_viewModel.FlatsCount}\nOpen LOKUM site?",
            //    "Lokum info update",
            //    MessageBoxButton.YesNo,
            //    MessageBoxImage.Exclamation);

            Utils.ScreenSaverPreventer.PreventScreenSaver(_viewModel.IsEnabled);

            _viewModel.UpdateDateTime = DateTime.Now;
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

        public void CloseApp()
        {
            TimerDispose();
            SystemEvents.SessionSwitch -= OnSessionSwitch;

            Application.Current.Shutdown();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            PreventSleep();
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionUnlock ||
                e.Reason == SessionSwitchReason.SessionLogon)
            {
                PreventSleep();
            }
        }

        private void EnabledStateChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Status = _viewModel.IsEnabled ? "Enabled" : "Disabled";
            btnToggle.Background = _viewModel.IsEnabled ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Gray;
        }
    }
}