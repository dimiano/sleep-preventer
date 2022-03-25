using System;
using System.Windows.Input;
using SleepPreventer.Commands;
using SleepPreventer.Views;

namespace SleepPreventer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IMainWindow _mainWindow;
        private DateTime _updateDateTime;
        private string _status;
        private bool _isEnabled;
        private int _updateInterval;

        private ICommand _showHideMainWindowCommand;
        private ICommand _updateInfoCommand;
        private ICommand _closeAppCommand;

        public MainViewModel(IMainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _updateDateTime = DateTime.Now;
            _isEnabled = true;
        }

        public DateTime UpdateDateTime
        {
            get => _updateDateTime;
            set => SetProperty(ref _updateDateTime, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value); // readonly
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public int UpdateInterval
        {
            get => _updateInterval;
            set => SetProperty(ref _updateInterval, value);
        }

        public ICommand CloseAppCommand => _closeAppCommand ?? (_closeAppCommand = new RelayCommand((o) => _mainWindow.CloseApp()));

        public ICommand UpdateInfoCommand => _updateInfoCommand ?? (_updateInfoCommand = new RelayCommand((o) => _mainWindow.PreventSleep()));

        public ICommand ShowHideMainWindowCommand =>
            _showHideMainWindowCommand ??
            (_showHideMainWindowCommand = new RelayCommand(o =>
            {
                var isForceShow = Convert.ToBoolean(o);
                _mainWindow.ToggleWindowVisibility(isForceShow);
            }));

    }
}