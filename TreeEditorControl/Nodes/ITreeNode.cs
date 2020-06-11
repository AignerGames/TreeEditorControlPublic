using System;

namespace TreeEditorControl.Nodes
{
    public interface ITreeNode
    {
        /// <summary>
        /// Invoked when the property of this node or the property of a child node changed.
        /// The <see cref="NodeChangedArgs"/> contain the original source node.
        /// </summary>
        event EventHandler<NodeChangedArgs> NodeChanged;

        string Header { get; }

        bool IsSelected { get; set; }

        ITreeNode Parent { get; }
    }
}
