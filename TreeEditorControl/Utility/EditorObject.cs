using System.Collections.Generic;

using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Utility
{
    public abstract class EditorObject : ObservableObject
    {
        public EditorObject(IEditorEnvironment editorEnvironment)
        {
            EditorEnvironment = editorEnvironment;
        }

        protected IEditorEnvironment EditorEnvironment { get; }

        protected IUndoRedoStack UndoRedoStack => EditorEnvironment.UndoRedoStack;

        protected UndoRedoValueWrapper<T> CreateUndoRedoWrapper<T>(string propertyName, T defaultValue = default)
        {
            return new UndoRedoValueWrapper<T>(UndoRedoStack, defaultValue, propertyName, NotifyUndoRedoPropertyChange);
        }

        protected UndoRedoListWrapper<T> CreateUndoRedoWrapper<T>(IList<T> list)
        {
            return new UndoRedoListWrapper<T>(UndoRedoStack, list);
        }

        protected virtual void NotifyUndoRedoPropertyChange(string propertyName)
        {
            NotifyPropertyChange(propertyName);
        }
    }
}
