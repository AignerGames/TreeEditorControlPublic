﻿
using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Example.Dialog
{
    [NodeCatalogInfo("ChoiceGroup", "Choices", "Group for choice items")]
    public class ChoiceGroup : DialogAction, ICopyableNode<ChoiceGroup>
    {
        private UndoRedoValueWrapper<string> _textUndoRedoWrapper;

        public ChoiceGroup(IEditorEnvironment editorEnvironment, string text = null) : base(editorEnvironment)
        {
            _textUndoRedoWrapper = CreateUndoRedoWrapper(nameof(Text), text);

            Choices = AddGroup<ChoiceItem>(nameof(Choices));

            UpdateHeader();
        }

        public string Text
        {
            get => _textUndoRedoWrapper.Value;
            set => _textUndoRedoWrapper.Value = value;
        }

        public TreeNodeContainer<ChoiceItem> Choices { get; }

        public ChoiceGroup CreateCopy()
        {
            var choiceCopy = new ChoiceGroup(EditorEnvironment, Text);

            choiceCopy.Choices.AddNodes(Choices.GetCopyableNodeCopies());

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
