using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;
using TreeEditorControl.Example.Dialog.Conditions;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("ConditionalAction", "Actions", "If / Else action")]
    public class ConditionalAction : DialogAction, ICopyableNode<ConditionalAction>
    {
        public ConditionalAction(IEditorEnvironment editorEnvironment) : base(editorEnvironment)
        {
            Conditions = new ConditionNodeContainer(EditorEnvironment);
            InsertChild(Conditions);


            TrueActions = AddGroup<DialogAction>(nameof(TrueActions));
            FalseActions = AddGroup<DialogAction>(nameof(FalseActions));
        }

        public ConditionNodeContainer Conditions { get; }

        public TreeNodeContainer<DialogAction> TrueActions { get; }

        public TreeNodeContainer<DialogAction> FalseActions { get; }

        public ConditionalAction CreateCopy()
        {
            var copy = new ConditionalAction(EditorEnvironment);

            copy.Conditions.CopyFrom(Conditions);
            copy.TrueActions.AddNodes(TrueActions.GetCopyableNodeCopies());
            copy.FalseActions.AddNodes(FalseActions.GetCopyableNodeCopies());

            return copy;
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitConditionalInteraction(this);
    }
}
