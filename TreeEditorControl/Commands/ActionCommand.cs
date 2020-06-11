using System;
using System.Windows.Input;

using TreeEditorControl.Utility;

namespace TreeEditorControl.Commands
{
    public class ActionCommand : ObservableObject, ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public ActionCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();
    }
}
