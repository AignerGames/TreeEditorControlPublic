using System.Collections.Generic;

namespace TreeEditorControl.UndoRedo.Implementation
{
    public class UndoRedoMultiCommand : IUndoRedoCommand
    {
        private readonly List<IUndoRedoCommand> _commands;

        public UndoRedoMultiCommand(IEnumerable<IUndoRedoCommand> commands)
        {
            _commands = new List<IUndoRedoCommand>(commands);
        }

        public void Redo()
        {
            foreach(var command in _commands)
            {
                command.Redo();
            }
        }

        public void Undo()
        {
            // Undo has to be handled in reverse order
            for(var i = _commands.Count - 1; i >= 0; --i)
            {
                _commands[i].Undo();
            }
        }
    }
}
