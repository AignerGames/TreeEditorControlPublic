using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("RemoveSceneObjectAction", "Scene", "Removes a object to the scene")]
    public class RemoveSceneObjectAction : DialogAction, ICopyableNode<RemoveSceneObjectAction>
    {
        private UndoRedoValueWrapper<string> _referenceName;

        public RemoveSceneObjectAction(IEditorEnvironment editorEnvironment, string referenceName = null)
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

        public RemoveSceneObjectAction CreateCopy()
        {
            return new RemoveSceneObjectAction(EditorEnvironment, ReferenceName);
        }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitRemoveSceneActor(this);

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            UpdateHeader();

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("RemoveSceneObjectAction", $"{ReferenceName}");
        }
    }
}
