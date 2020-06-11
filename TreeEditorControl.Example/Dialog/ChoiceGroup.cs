
using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog
{
    [NodeCatalogInfo("ChoiceGroup", "Choices", "Group for choice items")]
    public class ChoiceGroup : TreeNodeContainer<ChoiceItem>, IDialogAction, ICopyableNode<ChoiceGroup>
    {
        private UndoRedoValueWrapper<string> _textUndoRedoWrapper;

        public ChoiceGroup(IEditorEnvironment editorEnvironment, string text = null) : base(editorEnvironment)
        {
            _textUndoRedoWrapper = CreateUndoRedoWrapper(nameof(Text), text);

            UpdateHeader();

            // TODO: Maybe it would be better to inherit from ReadableGroupContainerNode instead of TreeNodeContainer<ChoiceItem>
            // And add a "Items" group, to make it more obvious that child items can be added.
        }

        public string Text
        {
            get => _textUndoRedoWrapper.Value;
            set => _textUndoRedoWrapper.Value = value;
        }

        public ChoiceGroup CreateCopy()
        {
            var choiceCopy = new ChoiceGroup(EditorEnvironment, Text);

            choiceCopy.AddNodes(GetCopyableNodeCopies());

            return choiceCopy;
        }

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            if (propertyName == nameof(Text))
            {
                UpdateHeader();
            }

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void UpdateHeader()
        {
            Header = DialogHelper.GetHeaderString("ChoiceGroup", Text);
        }
    }
}
