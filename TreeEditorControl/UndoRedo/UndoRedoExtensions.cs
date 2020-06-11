namespace TreeEditorControl.UndoRedo
{
    public static class UndoRedoExtensions
    {
        public static void ExecuteAndPush(this IUndoRedoStack stack, IUndoRedoCommand command)
        {
            command.Redo();
            stack.Push(command);
        }
    }
}
