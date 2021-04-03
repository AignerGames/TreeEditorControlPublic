using TreeEditorControl.Nodes;
using TreeEditorControl.UndoRedo;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Environment.Implementation
{
    public class EditorEnvironment : IEditorEnvironment
    {
        public EditorEnvironment(ITreeNodeFactory nodeFactory, IUndoRedoStack undoRedoStack)
        {
            NodeFactory = nodeFactory;
            UndoRedoStack = undoRedoStack;
        }

        public EditorEnvironment()
        {
            UndoRedoStack = new UndoRedoStack();
        }

        public EditorEnvironment(IEditorEnvironment editorEnvironment)
        {
            NodeFactory = editorEnvironment.NodeFactory;
            UndoRedoStack = editorEnvironment.UndoRedoStack;
        }

        public ITreeNodeFactory NodeFactory { get; set; }

        public IUndoRedoStack UndoRedoStack { get; set; }
    }
}
