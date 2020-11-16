using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("AddSceneObjectAction", "Scene", "Adds a object to the scene")]
    public class AddSceneObjectAction : DialogAction, ICopyableNode<AddSceneObjectAction>
    {
        private UndoRedoValueWrapper<string> _objectName;
        private UndoRedoValueWrapper<string> _referenceName;

        public AddSceneObjectAction(IEditorEnvironment editorEnvironment, string objectName = null, string referenceName = null) 
            : base(editorEnvironment)
        {
            _objectName = CreateUndoRedoWrapper(nameof(ObjectName), objectName);
            _referenceName = CreateUndoRedoWrapper(nameof(ReferenceName), referenceName);

            UpdateHeader();

            Position.PropertyChanged += (s, e) => UpdateHeader();
            Rotation.PropertyChanged += (s, e) => UpdateHeader();
        }

        public string ObjectName
        {
            get => _objectName.Value;
            set => _objectName.Value = value;
        }

        public string ReferenceName
        {
            get => _referenceName.Value;
            set => _referenceName.Value = value;
        }

        public Vector Position { get; } = new Vector();

        public Vector Rotation { get; } = new Vector();

        public AddSceneObjectAction CreateCopy()
        {
            var copy = new AddSceneObjectAction(EditorEnvironment, ObjectName, ReferenceName);

            copy.Position.CopyFrom(Position);
            copy.Rotation.CopyFrom(Rotation);

            return copy;
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitAddSceneActor(this);

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            UpdateHeader();

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("AddSceneObjectAction", $"{ObjectName} as {ReferenceName} Pos: {Position} Rot: {Rotation}");
        }
    }
}
