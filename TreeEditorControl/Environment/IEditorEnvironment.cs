using TreeEditorControl.Nodes;
using TreeEditorControl.UndoRedo;

namespace TreeEditorControl.Environment
{
    /// <summary>
    /// Access to editor "services".
    /// </summary>
    public interface IEditorEnvironment
    {
        ITreeNodeFactory NodeFactory { get; }

        IUndoRedoStack UndoRedoStack { get; }
    }
}
