using TreeEditorControl.Environment;

namespace TreeEditorControl.Nodes.Implementation
{
    public abstract class ReadableGroupContainerNode : ReadableNodeContainer<TreeNode>
    {
        public ReadableGroupContainerNode(IEditorEnvironment editorEnvironment) : base(editorEnvironment)
        {

        }

        protected TreeNodeContainer<T> AddGroup<T>(string groupName) where T : TreeNode
        {
            var groupContianer = new TreeNodeContainer<T>(EditorEnvironment, groupName);

            InsertChild(groupContianer);

            return groupContianer;
        }
    }
}
