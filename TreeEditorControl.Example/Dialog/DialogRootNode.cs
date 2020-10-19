using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;
using TreeEditorControl.Example.Dialog.Conditions;
using TreeEditorControl.Example.Dialog.Actions;

namespace TreeEditorControl.Example.Dialog
{
    public class DialogRootNode : DialogNode
    {
        private UndoRedoValueWrapper<string> _name;

        public DialogRootNode(IEditorEnvironment editorEnvironment, string name = "Dialog") : base(editorEnvironment)
        {
            _name = CreateUndoRedoWrapper(nameof(Name), name);

            Conditions = new ConditionNodeContainer(EditorEnvironment);
            InsertChild(Conditions);

            Actions = AddGroup<DialogAction>(nameof(Actions));

            UpdateHeader();
        }

        public string Name
        {
            get => _name.Value;
            set => _name.Value = value;
        }

        public ConditionNodeContainer Conditions { get; }

        public TreeNodeContainer<DialogAction> Actions { get; }

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            UpdateHeader();

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("Interaction", Name);
        }
    }
}
