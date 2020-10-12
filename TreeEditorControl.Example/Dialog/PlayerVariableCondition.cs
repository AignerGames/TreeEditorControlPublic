
using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;
using StoryCreator.Common.Interaction;

namespace TreeEditorControl.Example.Dialog
{
    [NodeCatalogInfo("PlayerVariableCondition", "Conditions", "Checks a specific player variable value")]
    public class PlayerVariableCondition : DialogCondition, ICopyableNode<PlayerVariableCondition>
    {
        private UndoRedoValueWrapper<string> _variable;
        private UndoRedoValueWrapper<ValueCompareKind> _compareKind;
        private UndoRedoValueWrapper<int> _compareValue;

        public PlayerVariableCondition(IEditorEnvironment editorEnvironment, string variableName = null, ValueCompareKind compareKind = ValueCompareKind.Equal, int variableValue = 0) : base(editorEnvironment)
        {
            _variable = CreateUndoRedoWrapper(nameof(Variable), variableName);
            _compareKind = CreateUndoRedoWrapper(nameof(CompareKind), compareKind);
            _compareValue = CreateUndoRedoWrapper(nameof(CompareValue), variableValue);

            UpdateHeader();
        }

        public string Variable
        {
            get => _variable.Value;
            set => _variable.Value = value;
        }

        public ValueCompareKind CompareKind
        {
            get => _compareKind.Value;
            set => _compareKind.Value = value;
        }

        public int CompareValue

        {
            get => _compareValue.Value;
            set => _compareValue.Value = value;
        }

        public PlayerVariableCondition CreateCopy()
        {
            return new PlayerVariableCondition(EditorEnvironment, Variable, CompareKind, CompareValue);
        }

        public override T Accept<T>(IDialogConditionVisitor<T> visitor) => visitor.VisitPlayerVariableCondition(this);

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            UpdateHeader();

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("VariableCondition", $"{Variable} {CompareKind} {CompareValue}");
        }
    }
}
