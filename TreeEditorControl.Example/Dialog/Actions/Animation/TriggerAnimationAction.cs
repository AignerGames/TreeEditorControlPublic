using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("TriggerAnimationAction", "Animation", "Triggers a animation")]
    public class TriggerAnimationAction : DialogAction, ICopyableNode<TriggerAnimationAction>
    {
        private UndoRedoValueWrapper<string> _referenceName;
        private UndoRedoValueWrapper<string> _triggerName;
        private UndoRedoValueWrapper<float> _waitUntilDoneTime;

        public TriggerAnimationAction(IEditorEnvironment editorEnvironment, string referenceName = null, string triggerName = null,
            float waitUntilDoneTime = 0) : base(editorEnvironment)
        {
            _referenceName = CreateUndoRedoWrapper(nameof(ReferenceName), referenceName);
            _triggerName = CreateUndoRedoWrapper(nameof(TriggerName), triggerName);
            _waitUntilDoneTime = CreateUndoRedoWrapper(nameof(WaitUntilDoneTime), waitUntilDoneTime);

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

        /// <summary>
        /// Normalized time (0-1), 0 means don't wait, 1 means wait until fully done.
        /// </summary>
        public float WaitUntilDoneTime
        {
            get => _waitUntilDoneTime.Value;
            set => _waitUntilDoneTime.Value = value;
        }

        public TriggerAnimationAction CreateCopy()
        {
            return new TriggerAnimationAction(EditorEnvironment, ReferenceName, TriggerName, WaitUntilDoneTime);
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitTriggerAnimation(this);

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            UpdateHeader();

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("TriggerAnimationAction", $"{ReferenceName}: {TriggerName} {WaitUntilDoneTime * 100}%");
        }
    }
}
