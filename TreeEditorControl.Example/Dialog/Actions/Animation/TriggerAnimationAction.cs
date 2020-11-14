using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("TriggerAnimationAction", "Animation", "Triggers a specific animation state")]
    public class TriggerAnimationAction : DialogAction, ICopyableNode<TriggerAnimationAction>
    {
        private UndoRedoValueWrapper<string> _referenceName;
        private UndoRedoValueWrapper<string> _triggerName;
        private UndoRedoValueWrapper<bool> _waitUntilDone;

        public TriggerAnimationAction(IEditorEnvironment editorEnvironment, string referenceName = null, string triggerName = null,
            bool waitUntilDone = false) : base(editorEnvironment)
        {
            _referenceName = CreateUndoRedoWrapper(nameof(ReferenceName), referenceName);
            _triggerName = CreateUndoRedoWrapper(nameof(TriggerName), triggerName);
            _waitUntilDone = CreateUndoRedoWrapper(nameof(WaitUntilDone), waitUntilDone);

            UpdateHeader();
        }

        public string ReferenceName
        {
            get => _referenceName.Value;
            set => _referenceName.Value = value;
        }

        public string TriggerName
        {
            get => _triggerName.Value;
            set => _triggerName.Value = value;
        }

        public bool WaitUntilDone
        {
            get => _waitUntilDone.Value;
            set => _waitUntilDone.Value = value;
        }

        public TriggerAnimationAction CreateCopy()
        {
            return new TriggerAnimationAction(EditorEnvironment, ReferenceName, TriggerName, WaitUntilDone);
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitTriggerAnimation(this);

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            UpdateHeader();

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("TriggerAnimationAction", $"{ReferenceName}: {TriggerName}");
        }
    }
}
