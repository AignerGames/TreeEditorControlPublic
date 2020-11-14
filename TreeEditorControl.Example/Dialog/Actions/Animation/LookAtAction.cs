using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("LookAtAction", "Animation", "Rotates the actor to the target slot")]
    public class LookAtAction : DialogAction, ICopyableNode<LookAtAction>
    {
        private UndoRedoValueWrapper<string> _actorSlotName;
        private UndoRedoValueWrapper<float> _duration;

        public LookAtAction(IEditorEnvironment editorEnvironment, string actorSlowName = null, float duration = default)
            : base(editorEnvironment)
        {
            _actorSlotName = CreateUndoRedoWrapper(nameof(ActorSlotName), actorSlowName);
            _duration = CreateUndoRedoWrapper(nameof(Duration), duration);

            UpdateHeader();
        }

        public string ActorSlotName
        {
            get => _actorSlotName.Value;
            set => _actorSlotName.Value = value;
        }

        public Vector TargetPosition { get; } = new Vector();

        public float Duration
        {
            get => _duration.Value;
            set => _duration.Value = value;
        }

        public LookAtAction CreateCopy()
        {
            var copy = new LookAtAction(EditorEnvironment, ActorSlotName, Duration);

            copy.TargetPosition.CopyFrom(TargetPosition);

            return copy;
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitLookAt(this);

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            UpdateHeader();

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("LookAtAction", $"{ActorSlotName}");
        }
    }
}
