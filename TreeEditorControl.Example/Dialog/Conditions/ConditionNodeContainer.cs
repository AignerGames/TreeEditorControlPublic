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
using StoryCreator.Common.Interaction;

namespace TreeEditorControl.Example.Dialog.Conditions
{
    public class ConditionNodeContainer : TreeNodeContainer<DialogCondition>
    {
        private readonly UndoRedoValueWrapper<MultiConditionKind> _condtionKind;

        public ConditionNodeContainer(IEditorEnvironment editorEnvironment, MultiConditionKind conditionKind = default) : base(editorEnvironment)
        {
            _condtionKind = CreateUndoRedoWrapper(nameof(ConditionKind), conditionKind);

            UpdateHeader();
        }

        public MultiConditionKind ConditionKind
        {
            get => _condtionKind.Value;
            set => _condtionKind.Value = value;
        }

        public void CopyFrom(ConditionNodeContainer source)
        {
            ConditionKind = source.ConditionKind;
            this.AddNodes(source.GetCopyableNodeCopies());
        }

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            UpdateHeader();

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("Conditions", ConditionKind.ToString());
        }
    }
}
