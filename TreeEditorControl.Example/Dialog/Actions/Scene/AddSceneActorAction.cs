using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("AddSceneActorAction", "Scene", "Adds a object to the scene")]
    public class AddSceneActorAction : DialogAction, ICopyableNode<AddSceneActorAction>
    {
        private UndoRedoValueWrapper<string> _objectName;
        private UndoRedoValueWrapper<string> _referenceName;

        public AddSceneActorAction(IEditorEnvironment editorEnvironment, string objectName = null, string referenceName = null) 
            : base(editorEnvironment)
        {
            _objectName = CreateUndoRedoWrapper(nameof(ObjectName), objectName);
            _referenceName = CreateUndoRedoWrapper(nameof(ReferenceName), referenceName);

            UpdateHeader();
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

        public Vector Rotation { get; } = new Vector(0, 180, 0);

        public AddSceneActorAction CreateCopy()
        {
            var copy = new AddSceneActorAction(EditorEnvironment, ObjectName, ReferenceName);

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
            Header = DialogHelper.GetHeaderString("AddSceneActorAction", $"{ObjectName} as {ReferenceName}");
        }
    }
}
