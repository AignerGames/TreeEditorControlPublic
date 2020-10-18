
using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog
{
    [NodeCatalogInfo("ChoiceItem", "Choices", "Option for a choice group")]
    public class ChoiceItem : DialogNode, ICopyableNode<ChoiceItem>
    {
        private UndoRedoValueWrapper<string> _textUndoRedoWrapper;

        public ChoiceItem(IEditorEnvironment editorEnvironment, string text = null) : base(editorEnvironment)
        {
            _textUndoRedoWrapper = CreateUndoRedoWrapper(nameof(Text), text);

            Conditions = new ConditionNodeContainer(editorEnvironment);
            InsertChild(Conditions);

            Actions = AddGroup<DialogAction>(nameof(Actions));

            UpdateHeader();
        }

        public string Text
        {
            get => _textUndoRedoWrapper.Value;
            set => _textUndoRedoWrapper.Value = value;
        }

        public ConditionNodeContainer Conditions { get; }

        public TreeNodeContainer<DialogAction> Actions { get; }

        public ChoiceItem CreateCopy()
        {
            var choiceCopy = new ChoiceItem(EditorEnvironment, Text);

            choiceCopy.Conditions.CopyFrom(Conditions);
            choiceCopy.Actions.AddNodes(Actions.GetCopyableNodeCopies());

            return choiceCopy;
        }

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            if (propertyName == nameof(Text))
            {
                UpdateHeader();
            }

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("ChoiceItem", Text);
        }
    }
}
