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

namespace TreeEditorControl.Example.Dialog
{
    [NodeCatalogInfo("DiceRollAction", "Actions", "Random success / fail action")]
    public class DiceRollAction : DialogAction, ICopyableNode<DiceRollAction>
    {
        private UndoRedoValueWrapper<int> _targetValue;
        private UndoRedoValueWrapper<int> _maxValue;

        public DiceRollAction(IEditorEnvironment editorEnvironment, int targetValue = 1, int maxValue = 6) : base(editorEnvironment)
        {
            _targetValue = CreateUndoRedoWrapper(nameof(TargetValue), targetValue);
            _maxValue = CreateUndoRedoWrapper(nameof(MaxValue), maxValue);

            SuccessActions = AddGroup<DialogAction>(nameof(SuccessActions));
            FailActions = AddGroup<DialogAction>(nameof(FailActions));

            UpdateHeader();
        }

        public int TargetValue
        {
            get => _targetValue.Value;
            set => _targetValue.Value = value;
        }

        public int MaxValue
        {
            get => _maxValue.Value;
            set
            {
                _maxValue.Value = Math.Max(1, value);
            }
        }

        public TreeNodeContainer<DialogAction> SuccessActions { get; }

        public TreeNodeContainer<DialogAction> FailActions { get; }

        public DiceRollAction CreateCopy()
        {
            var copy = new DiceRollAction(EditorEnvironment, TargetValue, MaxValue);

            copy.SuccessActions.AddNodes(SuccessActions.GetCopyableNodeCopies());
            copy.FailActions.AddNodes(FailActions.GetCopyableNodeCopies());

            return copy;
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitDiceRollAction(this);

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            UpdateHeader();

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("DiceRollAction", $"{TargetValue} 1-{MaxValue}");
        }
    }
}
