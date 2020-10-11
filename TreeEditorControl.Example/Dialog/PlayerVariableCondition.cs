
using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog
{
    [NodeCatalogInfo("PlayerVariableCondition", "Conditions", "Checks a specific player variable value")]
    public class PlayerVariableCondition : DialogCondition, ICopyableNode<PlayerVariableCondition>
    {
        private UndoRedoValueWrapper<string> _variableUndoRedoWrapper;
        private UndoRedoValueWrapper<int> _valueUndoRedoWrapper;

        public PlayerVariableCondition(IEditorEnvironment editorEnvironment, string variableName = null, int variableValue = 0) : base(editorEnvironment)
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

        public PlayerVariableCondition CreateCopy()
        {
            return new PlayerVariableCondition(EditorEnvironment, Variable, Value);
        }

        public override T Accept<T>(IDialogConditionVisitor<T> visitor) => visitor.VisitPlayerVariableCondition(this);

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
