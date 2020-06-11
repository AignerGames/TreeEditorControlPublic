using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using TreeEditorControl.Utility;

namespace TreeEditorControl.Commands
{
    public class ContextMenuCommand : ObservableObject, ICommand
    {
        private readonly Func<bool> _canShow;
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public ContextMenuCommand(string name, Func<bool> canShow, Action execute, Func<bool> canExecute, IEnumerable<ContextMenuCommand> subCommands = null)
        {
            Name = name;
            _canShow = canShow;
            _execute = execute;
            _canExecute = canExecute;

            var subCommandList = new ObservableCollection<ContextMenuCommand>(subCommands ?? Array.Empty<ContextMenuCommand>());

            SubCommands = new ReadOnlyObservableCollection<ContextMenuCommand>(subCommandList);
        }

        public ContextMenuCommand(string name, Func<bool> canShow, Action execute, IEnumerable<ContextMenuCommand> subCommands = null)
            : this(name, canShow, execute, null, subCommands)
        {

        }

        public ContextMenuCommand(string name, Action execute, Func<bool> canExecute, IEnumerable<ContextMenuCommand> subCommands = null)
            : this(name, null, execute, canExecute, subCommands)
        {

        }

        public ContextMenuCommand(string name, Action execute, IEnumerable<ContextMenuCommand> subCommands = null)
            : this(name, null, execute, null, subCommands)
        {

        }

        public event EventHandler CanExecuteChanged;

        public static ContextMenuCommand Seperator { get; } = new ContextMenuCommand(string.Empty, () => { });

        public string Name { get; }

        public ReadOnlyObservableCollection<ContextMenuCommand> SubCommands { get; }

        public bool CanShow() => _canShow == null || _canShow();

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();

        public void UpdateCanExecute()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
