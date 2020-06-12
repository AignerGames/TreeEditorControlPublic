using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;

namespace TreeEditorControl.Example.Dialog
{
    public class DialogRootNode : TreeNodeContainer<DialogAction>
    {
        public DialogRootNode(IEditorEnvironment editorEnvironment) : base(editorEnvironment, "Dialog")
        {

        }
    }
}
