using TreeEditorControl.Environment;

namespace TreeEditorControl.Example.Dialog
{
    public class DialogCondition : DialogNode
    {
        // Marker class
        // Can be used for common condition logic, for example serialization
        protected DialogCondition(IEditorEnvironment editorEnvironment) : base(editorEnvironment)
        {

        }
    }
}
