namespace TreeEditorControl.Nodes
{
    public interface ICopyableNode<out TNode> where TNode : ITreeNode
    {
        TNode CreateCopy();
    }
}
