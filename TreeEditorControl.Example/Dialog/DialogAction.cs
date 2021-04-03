using TreeEditorControl.Environment;

namespace TreeEditorControl.Example.Dialog
{
    public class DialogAction : DialogNode
    {
        // Marker class
        // Can be used for common action logic, for example serialization
        protected DialogAction(IEditorEnvironment editorEnvironment) : base(editorEnvironment)
        {

        }
    }
}
