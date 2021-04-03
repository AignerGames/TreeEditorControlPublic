using System;
using System.Collections.Generic;
using TreeEditorControl.Nodes.Implementation;

namespace TreeEditorControl.Nodes
{
    public static class NodeExtensions
    {
        public static bool TryAddNode(this ITreeNodeContainer container, ITreeNode node)
        {
            return container.TryInsertNode(container.Nodes.Count, node);
        }

        public static void AddNodes<T>(this TreeNodeContainer<T> container, IEnumerable<T> nodes) where T : TreeNode
        {
            foreach(var node in nodes)
            {
                container.Add(node);
            }
        }

        public static int IndexOf(this IReadableNodeContainer container, Predicate<ITreeNode> predicate)
        {
            for(var i = 0; i < container.Nodes.Count; ++i)
            {
                if(predicate(container.Nodes[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static int IndexOf(this IReadableNodeContainer container, ITreeNode node)
        {
            return container.IndexOf(n => ReferenceEquals(n, node));
        }

        public static bool TryRemoveNode(this ITreeNodeContainer container, ITreeNode node)
        {
            var index = container.IndexOf(n => ReferenceEquals(n, node));
            if(index < 0)
            {
                return false;
            }

            return container.TryRemoveNodeAt(index);
        }

        public static bool IsDescendantOf(this ITreeNode node, ITreeNode possibleParent)
        {
            for(var current = node; current != null; current = current.Parent)
            {
                if(ReferenceEquals(current, possibleParent))
                {
                    return true;
                }
            }

            return false;
        }

        public static void ExpandRecursive(this ITreeNode node)
        {
            if (node is IReadableNodeContainer container)
            {
                container.IsExpanded = true;

                foreach (var child in container.Nodes)
                {
                    ExpandRecursive(child);
                }
            }
        }

        public static T CreateNode<T>(this ITreeNodeFactory nodeFactory) where T : class, ITreeNode
        {
            return nodeFactory.CreateNode(typeof(T)) as T;
        }
    }
}
