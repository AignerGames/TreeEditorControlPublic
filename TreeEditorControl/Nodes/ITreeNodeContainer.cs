using System;

namespace TreeEditorControl.Nodes
{
    public interface ITreeNodeContainer : IReadableNodeContainer
    {
        bool IsNodeTypeSupported(Type nodeType);

        bool TryInsertNode(int index, ITreeNode node);

        void RemoveNodeAt(int index);
    }
}
