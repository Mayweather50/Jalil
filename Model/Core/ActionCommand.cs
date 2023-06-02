using System;
using System.Windows.Input;

namespace MySQLConnect.Core
{
    public class ActionCommand : ICommand
    {
        private Action action;

        public ActionCommand(Action action) { this.action = action; }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter) { this.action(); }
    }
}
