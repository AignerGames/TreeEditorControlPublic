using System;

namespace TreeEditorControl.Nodes
{
    public class NodeChangedArgs : EventArgs
    {
        public NodeChangedArgs(ITreeNode sourceNode, string propertyName)
        {
            SourceNode = sourceNode;
            PropertyName = propertyName;
        }

        /// <summary>
        /// The original source node of this event chain.
        /// </summary>
        public ITreeNode SourceNode { get; }

        /// <summary>
        /// The name of the changed property.
        /// </summary>
        public string PropertyName { get; }
    }
}
