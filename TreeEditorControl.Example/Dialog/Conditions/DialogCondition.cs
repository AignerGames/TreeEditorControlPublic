using TreeEditorControl.Environment;

namespace TreeEditorControl.Example.Dialog.Conditions
{
    public abstract class DialogCondition : DialogNode
    {
        // Marker class
        // Can be used for common condition logic, for example serialization
        protected DialogCondition(IEditorEnvironment editorEnvironment) : base(editorEnvironment)
        {

        }

        public abstract T Accept<T>(IDialogConditionVisitor<T> visitor);
    }
}
