using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("SequenceAction", "Parallel", "Actions inside a ParallelAction which will run in a sequence")]
    public class SequenceAction : DialogAction, ICopyableNode<SequenceAction>
    {
        public SequenceAction(IEditorEnvironment editorEnvironment) : base(editorEnvironment)
        {
            Actions = AddGroup<DialogAction>(nameof(Actions));
        }

        public TreeNodeContainer<DialogAction> Actions { get; }

        public SequenceAction CreateCopy()
        {
            var copy = new SequenceAction(EditorEnvironment);

            copy.Actions.AddNodes(Actions.GetCopyableNodeCopies());

            return copy;
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitSequenceAction(this);
    }
}
