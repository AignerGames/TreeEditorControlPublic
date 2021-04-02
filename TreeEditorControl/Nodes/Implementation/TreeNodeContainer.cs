using System;

using TreeEditorControl.Environment;

namespace TreeEditorControl.Nodes.Implementation
{
    public class TreeNodeContainer<T> : ReadableNodeContainer<T>, ITreeNodeContainer where T : TreeNode
    {
        public TreeNodeContainer(IEditorEnvironment editorEnvironment, string header = null) : base(editorEnvironment, header)
        {
        }

        public virtual bool CanInsertNode(Type nodeType)
        {
            return typeof(T).IsAssignableFrom(nodeType);
        }

        public virtual bool CanRemoveNode()
        {
            return true;
        }

        public bool TryInsertNode(int index, ITreeNode node)
        {
            if(node == null || !CanInsertNode(node.GetNodeType()))
            {
                return false;
            }

            if (!(node is T validNode))
            {
                return false;
            }

            if(this.IsDescendantOf(node))
            {
                return false;
            }

            InsertChild(validNode, index);

            return true;
        }

        public void Add(T node)
        {
            InsertChild(node);
        }

        public void Insert(int index, T node)
        {
            InsertChild(node, index);
        }

        public bool TryRemoveNodeAt(int index)
        {
            if(!CanRemoveNode())
            {
                return false;
            }

            RemoveNodeAt(index);

            return true;
        }


        public void RemoveNode(T node)
        {
            RemoveChild(node);
        }

        public void RemoveNodeAt(int index)
        {
            RemoveChild(index);
        }

        public void ClearNodes()
        {
            while(Nodes.Count > 0)
            {
                RemoveNodeAt(Nodes.Count - 1);
            }
        }
    }
}
