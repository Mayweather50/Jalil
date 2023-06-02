using System;
using System.Windows.Input;

namespace MySQLConnect.Core
{
    public class RelayCommand<T> : ICommand
    {
        private Action<T> action;
        private Func<T, bool> canExecute;

        public RelayCommand(Action<T> action, Func<T, bool> canExecute = null)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return this.canExecute == null || this.canExecute((T)parameter);
        }

        public void Execute(object? parameter)
        {
            this.action((T)parameter);
        }
    }
}
