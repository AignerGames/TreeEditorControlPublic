using TreeEditorControl.UndoRedo;

namespace TreeEditorControl.Environment
{
    /// <summary>
    /// Can be passed to all "editor objects" which need access to the <see cref="IUndoRedoStack"/>
    /// (And maybe more in the future, for example UserSettings).
    /// </summary>
    public interface IEditorEnvironment
    {
        /// TODO: Still not sure if this is a good design or not, because now every node needs a constructor
        /// with a <see cref="IEditorEnvironment"/> parameter, but the alternative would be to make it static or singleton 
        /// and that has other drawbacks. 
        /// With a DI framework it would also require a constructor parameter, so I guess it's fine?
        /// And the <see cref="Nodes.ITreeNodeFactory"/> can be used to "simplify" the construction of the nodes.

        IUndoRedoStack UndoRedoStack { get; }
    }
}
