using System;

namespace TreeEditorControl.UndoRedo.Implementation
{
    public class UndoRedoCommand : IUndoRedoCommand
    {
        private readonly Action _redoAction;
        private readonly Action _undoAction;

        public UndoRedoCommand(Action redoAction, Action undoAction)
        {
            _redoAction = redoAction;
            _undoAction = undoAction;
        }

        public void Redo() => _redoAction();

        public void Undo() => _undoAction();
    }
}
