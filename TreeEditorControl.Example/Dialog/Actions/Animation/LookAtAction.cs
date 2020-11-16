using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("LookAtAction", "Animation", "Rotates the object to the target position")]
    public class LookAtAction : DialogAction, ICopyableNode<LookAtAction>
    {
        private UndoRedoValueWrapper<string> _referenceName;
        private UndoRedoValueWrapper<float> _duration;

        public LookAtAction(IEditorEnvironment editorEnvironment, string referenceName = null, float duration = default)
            : base(editorEnvironment)
        {
            _referenceName = CreateUndoRedoWrapper(nameof(ReferenceName), referenceName);
            _duration = CreateUndoRedoWrapper(nameof(Duration), duration);

            UpdateHeader();

            TargetPosition.PropertyChanged += (s, e) => UpdateHeader();
        }

        public string ReferenceName
        {
            get => _referenceName.Value;
            set => _referenceName.Value = value;
        }

        public Vector TargetPosition { get; } = new Vector();

        public float Duration
        {
            get => _duration.Value;
            set => _duration.Value = value;
        }

        public LookAtAction CreateCopy()
        {
            var copy = new LookAtAction(EditorEnvironment, ReferenceName, Duration);

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
            Header = DialogHelper.GetHeaderString("LookAtAction", $"{ReferenceName} {TargetPosition} in {Duration} sec.");
        }
    }
}
