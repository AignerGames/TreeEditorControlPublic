using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("RemoveSceneActorAction", "Scene", "Removes a actor model to the scene")]
    public class RemoveSceneActorAction : DialogAction, ICopyableNode<RemoveSceneActorAction>
    {
        private UndoRedoValueWrapper<string> _actorSlotName;

        public RemoveSceneActorAction(IEditorEnvironment editorEnvironment, string actorSlowName = null)
            : base(editorEnvironment)
        {
            _actorSlotName = CreateUndoRedoWrapper(nameof(ActorSlotName), actorSlowName);

            UpdateHeader();
        }

        public string ActorSlotName
        {
            get => _actorSlotName.Value;
            set => _actorSlotName.Value = value;
        }

        public RemoveSceneActorAction CreateCopy()
        {
            return new RemoveSceneActorAction(EditorEnvironment, ActorSlotName);
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitRemoveSceneActor(this);

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            UpdateHeader();

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("RemoveSceneActorAction", $"At {ActorSlotName}");
        }
    }
}
