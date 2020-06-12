using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using TreeEditorControl.Environment;
using TreeEditorControl.UndoRedo;
using TreeEditorControl.UndoRedo.Implementation;

namespace TreeEditorControl.Nodes.Implementation
{
    public abstract class ReadableNodeContainer<T> : TreeNode, IReadableNodeContainer where T : TreeNode
    {
        /// <summary>
        /// Empty placeholder list for the <see cref="Nodes"/> property. 
        /// Used until the actual node collection is created
        /// </summary>
        private static readonly ReadOnlyObservableCollection<T> _emptyNodes = new ReadOnlyObservableCollection<T>(new ObservableCollection<T>());

        /// <summary>
        /// Lazy initialization, created when the first child is inserted
        /// </summary>
        private ObservableCollection<T> _nodes;

        private bool _isExpanded;

        public ReadableNodeContainer(IEditorEnvironment editorEnvironment, string header = null) : base(editorEnvironment, header)
        {
            Nodes = _emptyNodes;
        }

        public event EventHandler<TreeNodeEventArgs> NodeAdded;

        public event EventHandler<TreeNodeEventArgs> NodeRemoved;

        public bool IsExpanded { get => _isExpanded; set => SetAndNotify(ref _isExpanded, value); }

        public ReadOnlyObservableCollection<T> Nodes { get; private set; }

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
            AssureNodeListIsInitialized();

            if (index < 0)
            {
                index = _nodes.Count;
            }

            UndoRedoStack.ExecuteAndPush(new UndoRedoCommand(
                () => InsertChildCommandAction(index, child), 
                () => RemoveChildCommandAction(index)));
        }

        protected void RemoveChild(T child)
        {
            if(_nodes == null)
            {
                return;
            }

            var index = _nodes.IndexOf(child);

            RemoveChild(index);
        }

        protected void RemoveChild(int index)
        {
            if (index < 0)
            {
                return;
            }

            var child = _nodes[index];

            UndoRedoStack.ExecuteAndPush(new UndoRedoCommand(
                () => RemoveChildCommandAction(index),
                () => InsertChildCommandAction(index, child)));
        }

        /// <summary>
        /// Handles the lazy initialization of the <see cref="_nodes"/>.
        /// </summary>
        private void AssureNodeListIsInitialized()
        {
            if(_nodes != null)
            {
                return;
            }

            _nodes = new ObservableCollection<T>();
            Nodes = new ReadOnlyObservableCollection<T>(_nodes);
            NotifyPropertyChange(nameof(Nodes));
        }

        private void InsertChildCommandAction(int index, T chid)
        {
            SetChildParent(chid);
            _nodes.Insert(index, chid);

            NodeAdded?.Invoke(this, new TreeNodeEventArgs(chid));
        }

        private void RemoveChildCommandAction(int index)
        {
            var child = _nodes[index];

            RemoveChildParent(child);
            _nodes.RemoveAt(index);

            NodeRemoved?.Invoke(this, new TreeNodeEventArgs(child));
        }
    }
}
