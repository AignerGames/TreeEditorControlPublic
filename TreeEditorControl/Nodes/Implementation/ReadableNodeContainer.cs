using System.Collections.Generic;
using System.Collections.ObjectModel;

using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Nodes.Implementation
{
    public abstract class ReadableNodeContainer<T> : TreeNode, IReadableNodeContainer where T : class, ITreeNode
    {
        private bool _isExpanded;

        private readonly UndoRedoListWrapper<T> _nodesUndoRedoWrapper;

        public ReadableNodeContainer(IEditorEnvironment editorEnvironment, string header = null) : base(editorEnvironment, header)
        {
            var nodes = new ObservableCollection<T>();

            _nodesUndoRedoWrapper = CreateUndoRedoWrapper(nodes);
            Nodes = new ReadOnlyObservableCollection<T>(nodes);
        }

        public bool IsExpanded { get => _isExpanded; set => SetAndNotify(ref _isExpanded, value); }

        public IReadOnlyList<T> Nodes { get; }

        IReadOnlyList<ITreeNode> IReadableNodeContainer.Nodes => Nodes;

        /// <summary>
        /// Creates a copy of all child nodes which implement the <see cref="ICopyableNode{TNode}"/> interface.
        /// All other nodes are skipped.
        /// Compare the returned list count with the container node count to check if all nodes could be copied.
        /// </summary>
        public List<T> GetCopyableNodeCopies()
        {
            var copyNodes = new List<T>();

            foreach (var node in Nodes)
            {
                if (node is ICopyableNode<T> copyable)
                {
                    var copy = copyable.CreateCopy();
                    if(copy != null)
                    {
                        copyNodes.Add(copy);
                    }
                }
            }

            return copyNodes;
        }

        protected void InsertChild(T child, int index = -1)
        {
            var undoRedoId = UndoRedoStack.StartSequence();

            UndoRedoStack.ExecuteAndPush(new UndoRedoCommand(() => RegisterChild(child), () => DeregisterChild(child)));

            if (index < 0)
            {
                index = _nodesUndoRedoWrapper.Count;
            }

            _nodesUndoRedoWrapper.Insert(index, child);

            UndoRedoStack.EndSequence(undoRedoId);
        }

        protected void RemoveChild(T child)
        {
            var index = _nodesUndoRedoWrapper.IndexOf(child);

            RemoveChild(index);
        }

        protected void RemoveChild(int index)
        {
            if (index < 0)
            {
                return;
            }

            var undoRedoId = UndoRedoStack.StartSequence();

            var child = _nodesUndoRedoWrapper[index];
            UndoRedoStack.ExecuteAndPush(new UndoRedoCommand(() => DeregisterChild(child), () => RegisterChild(child)));

            _nodesUndoRedoWrapper.RemoveAt(index);

            UndoRedoStack.EndSequence(undoRedoId);
        }
    }
}
