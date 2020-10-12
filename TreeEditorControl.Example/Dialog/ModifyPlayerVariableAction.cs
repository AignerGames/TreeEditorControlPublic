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

namespace TreeEditorControl.Example.Dialog
{
    [NodeCatalogInfo("ModifyPlayerVariableAction", "Actions", "Checks a specific player variable value")]
    public class ModifyPlayerVariableAction : DialogAction, ICopyableNode<ModifyPlayerVariableAction>
    {
        private UndoRedoValueWrapper<string> _variable;
        private UndoRedoValueWrapper<ValueModifyKind> _modifyKind;
        private UndoRedoValueWrapper<int> _value;

        public ModifyPlayerVariableAction(IEditorEnvironment editorEnvironment, string variableName = null, ValueModifyKind modifyKind = ValueModifyKind.Set, int variableValue = 0) : base(editorEnvironment)
        {
            _variable = CreateUndoRedoWrapper(nameof(Variable), variableName);
            _modifyKind = CreateUndoRedoWrapper(nameof(ModifyKind), modifyKind);
            _value = CreateUndoRedoWrapper(nameof(Value), variableValue);

            UpdateHeader();
        }

        public string Variable
        {
            get => _variable.Value;
            set => _variable.Value = value;
        }

        public ValueModifyKind ModifyKind
        {
            get => _modifyKind.Value;
            set => _modifyKind.Value = value;
        }

        public int Value

        {
            get => _value.Value;
            set => _value.Value = value;
        }

        public ModifyPlayerVariableAction CreateCopy()
        {
            return new ModifyPlayerVariableAction(EditorEnvironment, Variable, ModifyKind, Value);
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitModifyPlayerVariable(this);

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            UpdateHeader();

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("ModifyPlayerVariableAction", $"{Variable} {ModifyKind} {Value}");
        }
    }
}
