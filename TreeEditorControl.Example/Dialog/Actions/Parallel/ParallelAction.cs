using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("ParallelAction", "Parallel", "Actions will run in parallel")]
    public class ParallelAction : DialogAction, ICopyableNode<ParallelAction>
    {
        public ParallelAction(IEditorEnvironment editorEnvironment) : base(editorEnvironment)
        {
            Actions = AddGroup<DialogAction>(nameof(Actions));
        }

        public TreeNodeContainer<DialogAction> Actions { get; }

        public ParallelAction CreateCopy()
        {
            var copy = new ParallelAction(EditorEnvironment);

            copy.Actions.AddNodes(Actions.GetCopyableNodeCopies());

            return copy;
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitParallelAction(this);
    }
}
