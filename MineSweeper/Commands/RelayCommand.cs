using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MineSweeper.Commands
{
    internal class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<object> execute, Predicate<object> predicate)
        {
            _execute = execute;
            _canExecute = predicate;
        }

        public bool CanExecute(object? parameter)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            return _canExecute(parameter);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        public void Execute(object? parameter)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            _execute(parameter);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        public static void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

    }
}
