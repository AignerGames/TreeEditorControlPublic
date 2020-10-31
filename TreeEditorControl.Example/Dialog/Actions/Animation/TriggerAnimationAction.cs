using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("TriggerAnimationAction", "Animation", "Triggers a specific animation state")]
    public class TriggerAnimationAction : DialogAction, ICopyableNode<TriggerAnimationAction>
    {
        private UndoRedoValueWrapper<string> _triggerName;
        private UndoRedoValueWrapper<string> _actorSlotName;
        private UndoRedoValueWrapper<bool> _waitUntilDone;

        public TriggerAnimationAction(IEditorEnvironment editorEnvironment, string triggerName = null, string actorSlowName = null,
            bool waitUntilDone = false) : base(editorEnvironment)
        {
            _triggerName = CreateUndoRedoWrapper(nameof(TriggerName), triggerName);
            _actorSlotName = CreateUndoRedoWrapper(nameof(ActorSlotName), actorSlowName);
            _waitUntilDone = CreateUndoRedoWrapper(nameof(WaitUntilDone), waitUntilDone);

            UpdateHeader();
        }

        public string TriggerName
        {
            get => _triggerName.Value;
            set => _triggerName.Value = value;
        }

        public string ActorSlotName
        {
            get => _actorSlotName.Value;
            set => _actorSlotName.Value = value;
        }

        public bool WaitUntilDone
        {
            get => _waitUntilDone.Value;
            set => _waitUntilDone.Value = value;
        }

        public TriggerAnimationAction CreateCopy()
        {
            return new TriggerAnimationAction(EditorEnvironment, TriggerName, ActorSlotName, WaitUntilDone);
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitTriggerAnimation(this);

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            UpdateHeader();

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("TriggerAnimationAction", $"{TriggerName} at {ActorSlotName}");
        }
    }
}
