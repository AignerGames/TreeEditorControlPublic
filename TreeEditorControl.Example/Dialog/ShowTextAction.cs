
using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog
{
    [NodeCatalogInfo("ShowText", "Actions", "Shows a dialog text")]
    public class ShowTextAction : TreeNode, IDialogAction, ICopyableNode<ShowTextAction>
    {
        private UndoRedoValueWrapper<string> _textUndoRedoWrapper;

        public ShowTextAction(IEditorEnvironment editorEnvironment, string text = null) : base(editorEnvironment)
        {
            _textUndoRedoWrapper = CreateUndoRedoWrapper(nameof(Text), text);

            UpdateHeader();
        }

        public string Text
        {
            get => _textUndoRedoWrapper.Value;
            set => _textUndoRedoWrapper.Value = value;
        }

        public ShowTextAction CreateCopy()
        {
            return new ShowTextAction(EditorEnvironment, Text);
        }

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            if(propertyName == nameof(Text))
            {
                UpdateHeader();
            }

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("ShowText", Text);
        }
    }
}
