namespace TreeEditorControl.Nodes
{
    /// <summary>
    /// The <see cref="ITreeNode.Parent"/> property doesn't have a setter,
    /// because the parent should be handled by the node classes.
    /// The <see cref="Implementation.TreeNode"/> has a protected method to
    /// set/remove the parent, but it can't set the parent of other <see cref="ITreeNode"/>
    /// implementations. This interface can be used if you want to mix the <see cref="Implementation.TreeNode"/>
    /// with custom <see cref="ITreeNode"/> implementations. 
    /// Usually it's not needed, because you can either derive all your custom nodes from <see cref="Implementation.TreeNode"/>
    /// or implement your own node class which handles the parent itself. 
    /// </summary>
    public interface ITreeNodeParentSettable
    {
        void SetParent(ITreeNode parent);
    }
}
