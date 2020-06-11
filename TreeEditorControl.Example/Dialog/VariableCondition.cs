
using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog
{
    [NodeCatalogInfo("VariableCondition", "Conditions", "Checks a specific variable value")]
    public class VariableCondition : TreeNode, IDialogCondition, ICopyableNode<VariableCondition>
    {
        private UndoRedoValueWrapper<string> _variableUndoRedoWrapper;
        private UndoRedoValueWrapper<int> _valueUndoRedoWrapper;

        public VariableCondition(IEditorEnvironment editorEnvironment, string variableName = null, int variableValue = 0) : base(editorEnvironment)
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

        public VariableCondition CreateCopy()
        {
            return new VariableCondition(EditorEnvironment, Variable, Value);
        }

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
            Header = DialogHelper.GetHeaderString("VariableCondition", $"{Variable} == {Value}");
        }
    }
}
