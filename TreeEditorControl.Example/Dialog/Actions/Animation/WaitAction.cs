using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("WaitAction", "Animation", "Waits for a specific duration (seconds)")]
    public class WaitAction : DialogAction, ICopyableNode<WaitAction>
    {
        private UndoRedoValueWrapper<float> _duration;

        public WaitAction(IEditorEnvironment editorEnvironment, float duration = default)
            : base(editorEnvironment)
        {
            _duration = CreateUndoRedoWrapper(nameof(Duration), duration);

            UpdateHeader();
        }

        public float Duration
        {
            get => _duration.Value;
            set => _duration.Value = value;
        }

        public WaitAction CreateCopy()
        {
            var copy = new WaitAction(EditorEnvironment, Duration);

            return copy;
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitWait(this);

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            UpdateHeader();

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("WaitAction", $"{Duration} sec");
        }
    }
}
