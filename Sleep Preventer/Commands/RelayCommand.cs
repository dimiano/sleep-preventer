﻿using System;
using System.Windows.Input;

namespace SleepPreventer.Commands;

public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Predicate<object> _canExecute;

    public RelayCommand(Action<object> execute)
        : this(execute, null)
    {
    }

    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    {
        ArgumentNullException.ThrowIfNull(execute);

        _execute = execute;
        _canExecute = canExecute;
    }

    public void Execute(object parameter) => _execute(parameter);

    public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

    public event EventHandler CanExecuteChanged;
    //{
    //    add { CommandManager.RequerySuggested += value; }
    //    remove { CommandManager.RequerySuggested -= value; }
    //}
}
