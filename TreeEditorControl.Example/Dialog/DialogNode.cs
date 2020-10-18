using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;

namespace TreeEditorControl.Example.Dialog
{
    public abstract class DialogNode : ReadableGroupContainerNode
    {
        // Marker class
        // Can be used for common dialog logic, for example serialization
        protected DialogNode(IEditorEnvironment editorEnvironment) : base(editorEnvironment)
        {

        }
    }
}
