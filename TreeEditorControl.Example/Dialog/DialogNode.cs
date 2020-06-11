using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;

namespace TreeEditorControl.Example.Dialog
{
    public class DialogNode : TreeNodeContainer<IDialogAction>
    {
        public DialogNode(IEditorEnvironment editorEnvironment) : base(editorEnvironment, "Dialog")
        {

        }
    }
}
