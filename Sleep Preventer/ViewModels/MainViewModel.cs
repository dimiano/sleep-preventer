﻿using System;
using System.Windows.Input;
using SleepPreventer.Commands;
using SleepPreventer.Utils;
using SleepPreventer.Views;

namespace SleepPreventer.ViewModels;

public class MainViewModel(IMainWindow mainWindow) : ViewModelBase
{
    private DateTime _updateDateTime = DateTime.Now;
    private string _status;
    private bool _isEnabled = true;
    private bool _isKeepTeamsOnline = true;
    private int _updateInterval;

    private ICommand _showHideMainWindowCommand;
    private ICommand _updateInfoCommand;
    private ICommand _closeAppCommand;

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

    public bool Enabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }
    
    public bool KeepTeamsOnline
    {
        get => _isKeepTeamsOnline;
        set => SetProperty(ref _isKeepTeamsOnline, value);
    }

    public int UpdateInterval
    {
        get => _updateInterval;
        set => SetProperty(ref _updateInterval, value);
    }

    public string[] KeyCodes => [.. Enum.GetNames<KeyboardPInvoke.KeyCode>()];
    public KeyboardPInvoke.KeyCode SelectedKey { get; set; }

    public string SelectedKeyItem
    {
        get => Enum.GetName(SelectedKey);
        set
        {
            SelectedKey = Enum.Parse<KeyboardPInvoke.KeyCode>(value);
        }
    }

    public ICommand CloseAppCommand => _closeAppCommand ??= new RelayCommand(o => mainWindow.CloseApp());

    public ICommand UpdateInfoCommand => _updateInfoCommand ??= new RelayCommand(o => mainWindow.UpdateSettings());

    public ICommand ShowHideMainWindowCommand => _showHideMainWindowCommand ??= new RelayCommand(o =>
    {
        var isForceShow = Convert.ToBoolean(o);
        mainWindow.ToggleWindowVisibility(isForceShow);
    });
}
