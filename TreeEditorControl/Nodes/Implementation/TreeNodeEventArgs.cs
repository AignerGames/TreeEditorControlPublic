using System;

namespace TreeEditorControl.Nodes.Implementation
{
    public class TreeNodeEventArgs : EventArgs
    {
        public TreeNodeEventArgs(TreeNode treeNode)
        {
            TreeNode = treeNode;
        }

        public TreeNode TreeNode { get; }
    }
}
