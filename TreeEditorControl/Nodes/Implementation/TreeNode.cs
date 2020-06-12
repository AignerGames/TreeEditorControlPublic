using System;

using TreeEditorControl.Environment;
using TreeEditorControl.Utility;

namespace TreeEditorControl.Nodes.Implementation
{
    public class TreeNode : EditorObject, ITreeNode
    {
        private string _header;
        private bool _isSelected;

        public TreeNode(IEditorEnvironment editorEnvironment, string header = null) : base(editorEnvironment)
        {
            Header = header ?? TypeUtility.GetTypeDisplayName(GetType());
        }

        public event EventHandler<NodeChangedArgs> NodeChanged;

        public string Header { get => _header; protected set => SetAndNotify(ref _header, value); }

        public bool IsSelected 
        { 
            get => _isSelected;
            set
            {
                if(value == true && Parent is IReadableNodeContainer parentContainer)
                {
                    parentContainer.IsExpanded = true;
                }

                SetAndNotify(ref _isSelected, value);
            }
        }

        public ITreeNode Parent { get; private set; }

        /// <summary>
        /// Sets the current node as the parent of the child node
        /// and handles the child <see cref="NodeChanged"/> event.
        /// </summary>
        /// <param name="child"></param>
        protected void RegisterChild(ITreeNode child)
        {
            SetChildParent(child);
            child.NodeChanged += Child_NodeChanged;
        }

        /// <summary>
        /// Removes the current node as the parent of the child node
        /// and removes the <see cref="NodeChanged"/> handler of the child.
        /// </summary>
        /// <param name="child"></param>
        protected void DeregisterChild(ITreeNode child)
        {
            child.NodeChanged -= Child_NodeChanged;
            RemoveChildParent(child);
        }

        protected void InvokeNodeChanged(NodeChangedArgs args)
        {
            NodeChanged?.Invoke(this, args);
        }

        protected override void NotifyPropertyChange(string propertyName)
        {
            base.NotifyPropertyChange(propertyName);

            InvokeNodeChanged(new NodeChangedArgs(this, propertyName));
        }

        protected override void NotifyUndoRedoPropertyChange(string propertyName)
        {
            // The node should be selected after undo/redo so it's easier to see which node changed
            IsSelected = true;

            base.NotifyUndoRedoPropertyChange(propertyName);
        }

        private void SetChildParent(ITreeNode child)
        {
            if (child.Parent != null)
            {
                throw new InvalidOperationException("The node parent is already set");
            }

            if (this.IsDescendantOf(child))
            {
                throw new InvalidOperationException($"Can't add parent node to a descendant child container node.");
            }

            if (child is TreeNode childNode)
            {
                childNode.Parent = this;
            }
            else if (child is ITreeNodeParentSettable parentSettable)
            {
                parentSettable.SetParent(this);
            }
            else
            {
                throw new NotSupportedException($"Can't set parent for node of type: {child.GetType()}");
            }
        }

        private void RemoveChildParent(ITreeNode child)
        {
            if (child.Parent != this)
            {
                throw new InvalidOperationException("The node doesn't belong to this parent");
            }

            if (child is TreeNode childNode)
            {
                childNode.Parent = null;
            }
            else if (child is ITreeNodeParentSettable parentSettable)
            {
                parentSettable.SetParent(null);
            }
            else
            {
                throw new NotSupportedException($"Can't remove parent for node of type: {child.GetType()}");
            }
        }

        private void Child_NodeChanged(object sender, NodeChangedArgs e)
        {
            InvokeNodeChanged(e);
        }

        public override string ToString() => $"{GetType().Name}  \"{Header}\"";
    }
}

