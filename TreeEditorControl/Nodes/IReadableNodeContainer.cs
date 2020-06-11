using System.Collections.Generic;

namespace TreeEditorControl.Nodes
{
    public interface IReadableNodeContainer : ITreeNode
    {
        bool IsExpanded { get; set; }

        IReadOnlyList<ITreeNode> Nodes { get; }
    }
}
