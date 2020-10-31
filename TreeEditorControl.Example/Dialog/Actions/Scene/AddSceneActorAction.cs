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
        private UndoRedoValueWrapper<string> _actorSlotName;
        private UndoRedoValueWrapper<string> _lookAtSlotName;

        public AddSceneActorAction(IEditorEnvironment editorEnvironment, string actor = null, 
            string actorSlowName = null, string lookAtSlotName = null) 
            : base(editorEnvironment)
        {
            _actor = CreateUndoRedoWrapper(nameof(Actor), actor);
            _actorSlotName = CreateUndoRedoWrapper(nameof(ActorSlotName), actorSlowName);
            _lookAtSlotName = CreateUndoRedoWrapper(nameof(LookAtSlotName), lookAtSlotName);

            UpdateHeader();
        }

        public string Actor
        {
            get => _actor.Value;
            set => _actor.Value = value;
        }

        public string ActorSlotName
        {
            get => _actorSlotName.Value;
            set => _actorSlotName.Value = value;
        }

        public string LookAtSlotName
        {
            get => _lookAtSlotName.Value;
            set => _lookAtSlotName.Value = value;
        }

        public AddSceneActorAction CreateCopy()
        {
            return new AddSceneActorAction(EditorEnvironment, Actor, ActorSlotName, LookAtSlotName);
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitAddSceneActor(this);

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            UpdateHeader();

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("AddSceneActorAction", $"{Actor} at {ActorSlotName}");
        }
    }
}
