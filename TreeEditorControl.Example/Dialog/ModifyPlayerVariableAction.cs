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
    [NodeCatalogInfo("ModifyPlayerVariableAction", "Actions", "Checks a specific player variable value")]
    public class ModifyPlayerVariableAction : DialogAction, ICopyableNode<ModifyPlayerVariableAction>
    {
        private UndoRedoValueWrapper<string> _variableUndoRedoWrapper;
        private UndoRedoValueWrapper<int> _valueUndoRedoWrapper;

        public ModifyPlayerVariableAction(IEditorEnvironment editorEnvironment, string variableName = null, int variableValue = 0) : base(editorEnvironment)
        {
            _variableUndoRedoWrapper = CreateUndoRedoWrapper(nameof(Variable), variableName);
            _valueUndoRedoWrapper = CreateUndoRedoWrapper(nameof(Value), variableValue);

            UpdateHeader();
        }

        public string Variable
        {
            get => _variableUndoRedoWrapper.Value;
            set => _variableUndoRedoWrapper.Value = value;
        }

        public int Value

        {
            get => _valueUndoRedoWrapper.Value;
            set => _valueUndoRedoWrapper.Value = value;
        }

        public ModifyPlayerVariableAction CreateCopy()
        {
            return new ModifyPlayerVariableAction(EditorEnvironment, Variable, Value);
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitModifyPlayerVariable(this);

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            if (propertyName == nameof(Variable) || propertyName == nameof(Value))
            {
                UpdateHeader();
            }

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("ModifyPlayerVariableAction", $"{Variable} == {Value}");
        }
    }
}
