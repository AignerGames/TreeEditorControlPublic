using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("RemoveSceneActorAction", "Scene", "Removes a actor model to the scene")]
    public class RemoveSceneActorAction : DialogAction, ICopyableNode<RemoveSceneActorAction>
    {
        private UndoRedoValueWrapper<string> _referenceName;

        public RemoveSceneActorAction(IEditorEnvironment editorEnvironment, string referenceName = null)
            : base(editorEnvironment)
        {
            _referenceName = CreateUndoRedoWrapper(nameof(ReferenceName), referenceName);

            UpdateHeader();
        }

        public string ReferenceName
        {
            get => _referenceName.Value;
            set => _referenceName.Value = value;
        }

        public RemoveSceneActorAction CreateCopy()
        {
            return new RemoveSceneActorAction(EditorEnvironment, ReferenceName);
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitRemoveSceneActor(this);

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            UpdateHeader();

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("RemoveSceneActorAction", $"{ReferenceName}");
        }
    }
}
