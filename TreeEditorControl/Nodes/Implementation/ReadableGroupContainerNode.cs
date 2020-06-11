using TreeEditorControl.Environment;

namespace TreeEditorControl.Nodes.Implementation
{
    public abstract class ReadableGroupContainerNode : ReadableNodeContainer<ITreeNode>
    {
        public ReadableGroupContainerNode(IEditorEnvironment editorEnvironment) : base(editorEnvironment)
        {

        }

        protected TreeNodeContainer<T> AddGroup<T>(string groupName) where T : class, ITreeNode
        {
            var groupContianer = new TreeNodeContainer<T>(EditorEnvironment, groupName);

            InsertChild(groupContianer);

            return groupContianer;
        }
    }
}
