using System;

namespace TreeEditorControl.Nodes
{
    public interface ITreeNodeContainer : IReadableNodeContainer
    {
        bool CanInsertNode(Type nodeType);

        bool CanRemoveNode();

        bool TryInsertNode(int index, ITreeNode node);

        bool TryRemoveNodeAt(int index);
    }
}
