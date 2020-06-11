using System;
using System.Windows.Input;

namespace TreeEditorControl.Commands
{
    public class EditorCommand : ActionCommand, ICommand
    {
        public EditorCommand(string name, string tooltip, Action execute, Func<bool> canExecute = null)
            : base(execute, canExecute)
        {
            Name = name;
            Tooltip = tooltip;
        }

        public string Name { get; }

        public string Tooltip { get; }
    }
}
