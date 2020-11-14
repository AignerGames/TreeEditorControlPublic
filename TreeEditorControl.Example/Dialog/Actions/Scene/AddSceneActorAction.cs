using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("AddSceneActorAction", "Scene", "Adds a actor model to the scene")]
    public class AddSceneActorAction : DialogAction, ICopyableNode<AddSceneActorAction>
    {
        private UndoRedoValueWrapper<string> _actor;

        public AddSceneActorAction(IEditorEnvironment editorEnvironment, string actor = null) 
            : base(editorEnvironment)
        {
            _actor = CreateUndoRedoWrapper(nameof(Actor), actor);

            UpdateHeader();
        }

        public string Actor
        {
            get => _actor.Value;
            set => _actor.Value = value;
        }

        public Vector Position { get; } = new Vector();

        public Vector Rotation { get; } = new Vector(0, 180, 0);

        public AddSceneActorAction CreateCopy()
        {
            var copy = new AddSceneActorAction(EditorEnvironment, Actor);

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
            Header = DialogHelper.GetHeaderString("AddSceneActorAction", $"{Actor}");
        }
    }
}
