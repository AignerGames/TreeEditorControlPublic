
using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("ShowText", "Actions", "Shows a dialog text")]
    public class ShowTextAction : DialogAction, ICopyableNode<ShowTextAction>
    {
        private UndoRedoValueWrapper<string> _actorUndoRedoWrapper;
        private UndoRedoValueWrapper<string> _textUndoRedoWrapper;

        public ShowTextAction(IEditorEnvironment editorEnvironment, string actor = null, string text = null) : base(editorEnvironment)
        {
            _actorUndoRedoWrapper = CreateUndoRedoWrapper(nameof(Actor), actor);
            _textUndoRedoWrapper = CreateUndoRedoWrapper(nameof(Text), text);

            UpdateHeader();
        }

        public string Actor
        {
            get => _actorUndoRedoWrapper.Value;
            set => _actorUndoRedoWrapper.Value = value;
        }

        public string Text
        {
            get => _textUndoRedoWrapper.Value;
            set => _textUndoRedoWrapper.Value = value;
        }

        public ShowTextAction CreateCopy()
        {
            return new ShowTextAction(EditorEnvironment, Actor, Text);
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitShowText(this);

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            if (propertyName == nameof(Text) || propertyName == nameof(Actor))
            {
                UpdateHeader();
            }

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("ShowText", $"{Actor}: {Text}");
        }
    }
}
