using TreeEditorControl.UndoRedo;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Environment.Implementation
{
    public class EditorEnvironment : IEditorEnvironment
    {
        public EditorEnvironment(IUndoRedoStack undoRedoStack)
        {
            UndoRedoStack = undoRedoStack;
        }

        public EditorEnvironment()
        {
            UndoRedoStack = new UndoRedoStack();
        }

        public IUndoRedoStack UndoRedoStack { get; }
    }
}
