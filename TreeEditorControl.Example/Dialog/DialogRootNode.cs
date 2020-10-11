using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog
{
    public class DialogRootNode : TreeNodeContainer<DialogAction>
    {
        private UndoRedoValueWrapper<string> _nameUndoRedoWrapper;

        public DialogRootNode(IEditorEnvironment editorEnvironment, string name = "Dialog") : base(editorEnvironment, name)
        {
            _nameUndoRedoWrapper = CreateUndoRedoWrapper(nameof(Name), name);

            UpdateHeader();
        }

        public string Name
        {
            get => _nameUndoRedoWrapper.Value;
            set => _nameUndoRedoWrapper.Value = value;
        }

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            if (propertyName == nameof(Name))
            {
                UpdateHeader();
            }

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("Interaction", Name);
        }
    }
}
