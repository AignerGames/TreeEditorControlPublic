namespace TreeEditorControl.UndoRedo
{
    public interface IUndoRedoCommand
    {
        void Undo();

        void Redo();
    }
}
