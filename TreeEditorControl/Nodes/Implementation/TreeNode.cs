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

        public TreeNode Parent { get; private set; }

        ITreeNode ITreeNode.Parent => Parent;

        protected void SetChildParent(TreeNode child)
        {
            if (child.Parent != null)
            {
                throw new InvalidOperationException("The node parent is already set");
            }

            if (this.IsDescendantOf(child))
            {
                throw new InvalidOperationException($"Can't add parent node to a descendant child container node.");
            }

            child.Parent = this;
        }

        protected void RemoveChildParent(TreeNode child)
        {
            if (child.Parent != this)
            {
                throw new InvalidOperationException("The node doesn't belong to this parent");
            }

            child.Parent = null;
        }

        /// <summary>
        /// Invokes the <see cref="NodeChanged"/> for this node and all parent nodes.
        /// </summary>
        /// <param name="args"></param>
        protected void InvokeNodeChanged(NodeChangedArgs args)
        {
            for(var currentNode = this; currentNode != null; currentNode = currentNode.Parent)
            {
                currentNode.NodeChanged?.Invoke(this, args);
            }
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

        public override string ToString() => $"{GetType().Name}  \"{Header}\"";
    }
}

