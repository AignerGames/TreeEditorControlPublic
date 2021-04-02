using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Data;

using TreeEditorControl.Catalog;
using TreeEditorControl.Commands;
using TreeEditorControl.Environment;
using TreeEditorControl.Nodes;
using TreeEditorControl.Utility;
using TreeEditorControl.UndoRedo.Implementation;
using TreeEditorControl.UndoRedo;
using System.Collections.Generic;

namespace TreeEditorControl.ViewModel
{
    // TODO: Refactoring, maybe some of the command stuff can be exported to a command handler class

    public class TreeEditorViewModel : EditorObject, IContextMenuOpeningHandler,
        IDragDropHandler<ITreeNode, ITreeNode>, IDragDropHandler<NodeCatalogItem, ITreeNode>
    {
        private readonly ObservableCollection<ITreeNode> _rootNodes = new ObservableCollection<ITreeNode>();

        private NodeCatalogItem _selectedCatalogItem;

        private ITreeNode _selectedNode;

        private ObservableCollection<ContextMenuCommand> _activeContextMenuCommands = new ObservableCollection<ContextMenuCommand>();

        /// <summary>
        /// The System.Windows.Clipboard uses serialization.
        /// This means all node classes would need the <see cref="SerializableAttribute"/> otherwise the clipboard doesn't work.
        /// Instead of dealing with serialization I decided to use this simple workaround and store node copy in a field.
        /// This means that Copy&Paste will only work inside the same tree, but better than nothing.
        /// </summary>
        private ITreeNode _clipboardNode;

        public TreeEditorViewModel(IEditorEnvironment editorEnvironment) : base(editorEnvironment)
        {
            RootNodes = new ReadOnlyObservableCollection<ITreeNode>(_rootNodes);

            // These commands are used for keyboard shortcuts
            DeleteNodeCommand = new ActionCommand(DeleteSelectedNode, CanDeletedSelectedNode);
            CutNodeCommand = new ActionCommand(CutNode, CanCutNode);
            CopyNodeCommand = new ActionCommand(CopyNode, CanCopyNode);
            PasteNodeCommand = new ActionCommand(PasteNode, CanPasteNode);
            DuplicateNodeCommand = new ActionCommand(DuplicateNode, CanDuplicateNode);
            UndoCommand = new ActionCommand(Undo, CanUndo);
            RedoCommand = new ActionCommand(Redo, CanRedo);

            ActiveContextMenuCommands = new ReadOnlyObservableCollection<ContextMenuCommand>(_activeContextMenuCommands);

            // Group catalog items by category
            var view = CollectionViewSource.GetDefaultView(CatalogItems);
            var groupDescription = new PropertyGroupDescription(nameof(NodeCatalogItem.Category));
            view.GroupDescriptions.Add(groupDescription);
        }

        /// <summary>
        /// Invoked when the property of any node in the tree changed.
        /// </summary>
        public event EventHandler<NodeChangedArgs> NodeChanged;

        public ObservableCollection<NodeCatalogItem> CatalogItems { get; } = new ObservableCollection<NodeCatalogItem>();

        public ReadOnlyObservableCollection<ITreeNode> RootNodes { get; }

        public ObservableCollection<EditorCommand> Commands { get; } = new ObservableCollection<EditorCommand>();

        public List<ContextMenuCommand> ContextMenuCommands { get; } = new List<ContextMenuCommand>();

        public ReadOnlyObservableCollection<ContextMenuCommand> ActiveContextMenuCommands { get; }

        public NodeCatalogItem SelectedCatalogItem { get => _selectedCatalogItem; set => SetAndNotify(ref _selectedCatalogItem, value); }
        
        public ITreeNode SelectedNode { get => _selectedNode; private set => SetAndNotify(ref _selectedNode, value); }

        public ActionCommand DeleteNodeCommand { get; }

        public ActionCommand CutNodeCommand { get; }

        public ActionCommand CopyNodeCommand { get; }

        public ActionCommand PasteNodeCommand { get; }

        public ActionCommand DuplicateNodeCommand { get; }

        public ActionCommand UndoCommand { get; }

        public ActionCommand RedoCommand { get; }

        public void AddRootNode(ITreeNode node, int index = -1)
        {
            if (node == null)
            {
                return;
            }

            if(index == -1)
            {
                index = _rootNodes.Count;
            }

            _rootNodes.Insert(index, node);
            node.NodeChanged += Node_NodeChanged;
        }

        public void AddRootNodes(IEnumerable<ITreeNode> nodes)
        {
            foreach(var node in nodes)
            {
                AddRootNode(node);
            }
        }

        public void RemoveRootNode(ITreeNode node)
        {
            if(node == null)
            {
                return;
            }

            node.NodeChanged -= Node_NodeChanged;
            _rootNodes.Remove(node);
        }

        public void ClearRootNodes()
        {
            foreach(var node in _rootNodes)
            {
                node.NodeChanged -= Node_NodeChanged;
            }

            _rootNodes.Clear();

            SelectedNode = null;
        }

        /// <summary>
        /// Adds the default commands for node operations (Add, Insert, Append, Replace, Delete, MoveUp, MoveDown) and Undo/Redo.
        /// The default commands are not required and can be replaced by custom commands with the public CanX() and X() methods.
        /// For example; The "Add" command uses CanAddCatalogNode() and AddCatalogNode()
        /// </summary>
        public void AddDefaultCommands()
        {
            Commands.Add(new EditorCommand("Add", "Adds the selected catalog item", AddCatalogNode, CanAddCatalogNode));
            Commands.Add(new EditorCommand("Insert", "Inserts the selected catalog item", () => InsertCatalogNode(0), () => CanInsertCatalogNode(0)));
            Commands.Add(new EditorCommand("Append", "Appends the selected catalog item", () => InsertCatalogNode(1), () => CanInsertCatalogNode(1)));
            Commands.Add(new EditorCommand("Replace", "Replaces the selected node with the selected catalog item", ReplaceCatalogNode, CanReplaceCatalogNode));
            Commands.Add(new EditorCommand("Delete", "Deletes the selected node", DeleteSelectedNode, CanDeletedSelectedNode));
            Commands.Add(new EditorCommand("MoveUp", "Moves the selected node up", () => Move(-1), () => CanMove(-1)));
            Commands.Add(new EditorCommand("MoveDown", "Moves the selected node down", () => Move(1), () => CanMove(1)));

            Commands.Add(new EditorCommand("Undo", "Undo the last action", Undo, CanUndo));
            Commands.Add(new EditorCommand("Redo", "Redo the last action", Redo, CanRedo));
        }

        /// <summary>
        /// Adds the default commands for node operations (Move up, Move down, Cut, Copy, Paste, Undo, Redo)
        /// </summary>
        public void AddDefaultContextMenuCommands()
        {
            ContextMenuCommands.Add(new ContextMenuCommand("Move up", () => CanMove(-1), () => Move(-1), () => CanMove(-1)));
            ContextMenuCommands.Add(new ContextMenuCommand("Move down", () => CanMove(1), () => Move(1), () => CanMove(1)));

            ContextMenuCommands.Add(ContextMenuCommand.Seperator);

            ContextMenuCommands.Add(new ContextMenuCommand("Cut", CutNode, CanCutNode));
            ContextMenuCommands.Add(new ContextMenuCommand("Copy", CopyNode, CanCopyNode));
            ContextMenuCommands.Add(new ContextMenuCommand("Paste", PasteNode, CanPasteNode));
            ContextMenuCommands.Add(new ContextMenuCommand("Duplicate", DuplicateNode, CanDuplicateNode));

            ContextMenuCommands.Add(ContextMenuCommand.Seperator);

            ContextMenuCommands.Add(new ContextMenuCommand("Undo", Undo, CanUndo));
            ContextMenuCommands.Add(new ContextMenuCommand("Redo", Redo, CanRedo));
        }

        public bool CanAddCatalogNode()
        {
            return VerifyCatalogNodeContainerType(SelectedNode);
        }

        public void AddCatalogNode()
        {
            if(!CanAddCatalogNode())
            {
                return;
            }

            var container = (ITreeNodeContainer)SelectedNode;

            var undoRedoId = UndoRedoStack.StartSequence();

            CreateAndInsertSelectedItem(container, container.Nodes.Count);

            UndoRedoStack.EndSequence(undoRedoId);
        }

        public bool CanInsertCatalogNode(int insertOffset)
        {
            if(!VerifyCatalogNodeContainerType(SelectedNode?.Parent))
            {
                return false;
            }

            if (!(SelectedNode?.Parent is ITreeNodeContainer container))
            {
                return false;
            }

            var selectedNodeIndex = container.IndexOf(SelectedNode);
            if (selectedNodeIndex < 0)
            {
                return false;
            }

            var insertIndex = selectedNodeIndex + insertOffset;
            return insertIndex >= 0 && insertIndex <= container.Nodes.Count;
        }

        public void InsertCatalogNode(int insertOffset)
        {
            if(!CanInsertCatalogNode(insertOffset))
            {
                return;
            }

            var container = (ITreeNodeContainer)SelectedNode.Parent;

            var selectedNodeIndex = container.IndexOf(SelectedNode);
            if(selectedNodeIndex < 0)
            {
                return;
            }

            var undoRedoId = UndoRedoStack.StartSequence();

            var insertIndex = selectedNodeIndex + insertOffset;

            CreateAndInsertSelectedItem(container, insertIndex);

            UndoRedoStack.EndSequence(undoRedoId);
        }

        public bool CanReplaceCatalogNode()
        {
            return VerifyCatalogNodeContainerType(SelectedNode?.Parent) && 
                SelectedNode?.Parent is ITreeNodeContainer parentContainer && parentContainer.CanRemoveNode();
        }

        public void ReplaceCatalogNode()
        {
            if(!CanReplaceCatalogNode())
            {
                return;
            }

            var container = (ITreeNodeContainer)SelectedNode.Parent;

            var selectedNodeIndex = container.IndexOf(SelectedNode);
            if (selectedNodeIndex < 0)
            {
                return;
            }

            var undoRedoId = UndoRedoStack.StartSequence();

            if(container.TryRemoveNodeAt(selectedNodeIndex))
            {
                CreateAndInsertSelectedItem(container, selectedNodeIndex);
            }

            UndoRedoStack.EndSequence(undoRedoId);
        }

        public bool CanDeletedSelectedNode()
        {
            return SelectedNode?.Parent is ITreeNodeContainer parentContainer && parentContainer.CanRemoveNode();
        }

        public void DeleteSelectedNode()
        {
            if(!CanDeletedSelectedNode())
            {
                return;
            }

            var container = (ITreeNodeContainer)SelectedNode.Parent;

            var selectedNodeIndex = container.IndexOf(SelectedNode);
            if (selectedNodeIndex < 0)
            {
                return;
            }

            var undoRedoId = UndoRedoStack.StartSequence();

            var newSelection = container.Nodes.ElementAtOrDefault(selectedNodeIndex + 1);
            if (newSelection == null)
            {
                newSelection = container.Nodes.ElementAtOrDefault(selectedNodeIndex - 1);
                if (newSelection == null)
                {
                    newSelection = container;
                }
            }


            AddChangeNodeSelectionUndoRedoCommand(newSelection);

            container.TryRemoveNodeAt(selectedNodeIndex);

            UndoRedoStack.EndSequence(undoRedoId);
        }

        public bool CanMove(int indexOffset)
        {
            if(!(SelectedNode?.Parent is ITreeNodeContainer parentContainer) ||
                !parentContainer.CanInsertNode(SelectedNode.GetNodeType()) || !parentContainer.CanRemoveNode())
            {
                return false;
            }

            var currentIndex = parentContainer.IndexOf(SelectedNode);
            if(currentIndex < 0)
            {
                return false;
            }

            var newIndex = currentIndex + indexOffset;

            return newIndex >= 0 && newIndex < parentContainer.Nodes.Count;

        }

        public void Move(int indexOffset)
        {
            if(!CanMove(indexOffset))
            {
                return;
            }

            var movingNode = SelectedNode;     
            var parentContainer = (ITreeNodeContainer)movingNode.Parent;
            var currentIndex = parentContainer.IndexOf(movingNode);
            var newIndex = currentIndex  + indexOffset;

            var undoRedoId = UndoRedoStack.StartSequence();

            parentContainer.IsSelected = true;

            var wasRemoved = parentContainer.TryRemoveNodeAt(currentIndex);
            if(wasRemoved && parentContainer.TryInsertNode(newIndex, movingNode))
            {
                AddChangeNodeSelectionUndoRedoCommand(movingNode);
            }

            UndoRedoStack.EndSequence(undoRedoId);
        }

        public bool CanUndo() => UndoRedoStack.CanUndo;

        public void Undo()
        {
            if(!CanUndo())
            {
                return;
            }

            UndoRedoStack.Undo();
        }

        public bool CanRedo() => UndoRedoStack.CanRedo;

        public void Redo()
        {
            if(!CanRedo())
            {
                return;
            }

            UndoRedoStack.Redo();
        }

        public bool CanDrag(ITreeNode dragNode)
        {
            return dragNode?.Parent is ITreeNodeContainer;
        }

        public bool CanDrop(ITreeNode sourceNode, ITreeNode targetNode, DropLocation dropLocation)
        {
            var actualDropContainer = GetDropContainerByLocation(targetNode, dropLocation);

            if (sourceNode == null || actualDropContainer == null || ReferenceEquals(sourceNode.Parent, actualDropContainer)
                 || actualDropContainer.IsDescendantOf(sourceNode))
            {
                return false;
            }

            if(!(sourceNode?.Parent is ITreeNodeContainer sourceContainer && sourceContainer.CanRemoveNode()))
            {
                return false;
            }

            return actualDropContainer.CanInsertNode(sourceNode.GetNodeType());
        }

        public void Drop(ITreeNode sourceNode, ITreeNode targetNode, DropLocation dropLocation)
        {
            if (!CanDrop(sourceNode, targetNode, dropLocation))
            {
                return;
            }

            if (!(sourceNode?.Parent is ITreeNodeContainer sourceContainer))
            {
                return;
            }

            var (dropTargetContainer, insertIndex) = GetDropInserInfoByLocation(targetNode, dropLocation);
            if (dropTargetContainer == null)
            {
                return;
            }

            var sourceNodeIndex = sourceContainer.IndexOf(sourceNode);
            if (sourceNodeIndex < 0)
            {
                return;
            }

            var undoRedoId = UndoRedoStack.StartSequence();

            var wasRemoved = sourceContainer.TryRemoveNodeAt(sourceNodeIndex);
            if (wasRemoved && dropTargetContainer.TryInsertNode(insertIndex, sourceNode))
            {
                AddChangeNodeSelectionUndoRedoCommand(sourceNode);
            }

            UndoRedoStack.EndSequence(undoRedoId);
        }

        public bool CanDrag(NodeCatalogItem dragItem)
        {
            return dragItem?.NodeType != null;
        }

        public bool CanDrop(NodeCatalogItem dragItem, ITreeNode targetNode, DropLocation dropLocation)
        {
            var actualDropContainer = GetDropContainerByLocation(targetNode, dropLocation);
            return dragItem?.NodeType != null && actualDropContainer != null && actualDropContainer.CanInsertNode(dragItem.NodeType);
        }

        public void Drop(NodeCatalogItem dragItem, ITreeNode targetNode, DropLocation dropLocation)
        {
            if(!CanDrop(dragItem, targetNode, dropLocation))
            {
                return;
            }

            var (dropTargetContainer, insertIndex) = GetDropInserInfoByLocation(targetNode, dropLocation);
            if(dropTargetContainer == null)
            {
                return;
            }

            // Select the container for undo/redo, so the container will be selected after undo
            dropTargetContainer.IsSelected = true;

            var undoRedoId = UndoRedoStack.StartSequence();

            CreateAndInsertNodeCatalogItem(dropTargetContainer, insertIndex, dragItem);

            UndoRedoStack.EndSequence(undoRedoId);
        }

        public bool CanCutNode()
        {
            return SelectedNode is ICopyableNode<ITreeNode> && SelectedNode?.Parent is ITreeNodeContainer parentContainer && parentContainer.CanRemoveNode();
        }

        public void CutNode()
        {
            if(!CanCutNode())
            {
                return;
            }

            var copyable = (ICopyableNode<ITreeNode>)SelectedNode;
            var container = (ITreeNodeContainer)SelectedNode.Parent;

            var selectedNodeIndex = container.IndexOf(SelectedNode);
            if (selectedNodeIndex < 0)
            {
                return;
            }

            var undoRedoId = UndoRedoStack.StartSequence();

            AddChangeNodeSelectionUndoRedoCommand(container);

            container.TryRemoveNodeAt(selectedNodeIndex);

            UndoRedoStack.EndSequence(undoRedoId);

            _clipboardNode = CreateNodeCopy(copyable);
        }

        public bool CanCopyNode()
        {
            return SelectedNode is ICopyableNode<ITreeNode>;
        }

        public void CopyNode()
        {
            if(!CanCopyNode())
            {
                _clipboardNode = null;
                return;
            }

            var copyable = (ICopyableNode<ITreeNode>)SelectedNode;

            _clipboardNode = CreateNodeCopy(copyable);
        }

        public bool CanPasteNode()
        {
            return SelectedNode is ITreeNodeContainer && _clipboardNode != null;
        }

        public void PasteNode()
        {
            if(!CanPasteNode())
            {
                return;
            }

            var container = (ITreeNodeContainer)SelectedNode;

            var undoRedoId = UndoRedoStack.StartSequence();

            InsertNode(container, container.Nodes.Count, _clipboardNode);

            UndoRedoStack.EndSequence(undoRedoId);

            // Create a new copy, this allows multiple paste actions for the "same" node
            _clipboardNode = CreateNodeCopy(_clipboardNode as ICopyableNode<ITreeNode>);
        }

        public bool CanDuplicateNode()
        {
            return SelectedNode is ICopyableNode<ITreeNode> && SelectedNode?.Parent is ITreeNodeContainer;
        }

        public void DuplicateNode()
        {
            if(!CanDuplicateNode())
            {
                return;
            }

            var copyable = (ICopyableNode<ITreeNode>)SelectedNode;
            var container = (ITreeNodeContainer)SelectedNode.Parent;

            var selectedNodeIndex = container.IndexOf(SelectedNode);
            if (selectedNodeIndex < 0)
            {
                return;
            }

            var undoRedoId = UndoRedoStack.StartSequence();

            var duplicateNode = CreateNodeCopy(copyable);

            if(container.TryInsertNode(selectedNodeIndex + 1, duplicateNode))
            {
                AddChangeNodeSelectionUndoRedoCommand(duplicateNode);
            }

            UndoRedoStack.EndSequence(undoRedoId);
        }

        private void Node_NodeChanged(object sender, NodeChangedArgs e)
        {
            // Update the selection, only one node can be selected
            if(e.PropertyName == nameof(ITreeNode.IsSelected))
            {
                if(e.SourceNode.IsSelected)
                {
                    SelectedNode = e.SourceNode;
                }
                else if(ReferenceEquals(SelectedNode, e.SourceNode))
                {
                    SelectedNode = null;
                }
            }

            NodeChanged?.Invoke(this, e);
        }

        private bool VerifyCatalogNodeContainerType(ITreeNode node)
        {
            var nodeType = SelectedCatalogItem?.NodeType;
            if (nodeType == null)
            {
                return false;
            }

            return node is ITreeNodeContainer container && container.CanInsertNode(nodeType);
        }

        private ITreeNode CreateCatalogNode(NodeCatalogItem catalogItem)
        {
            if(catalogItem == null)
            {
                return null;
            }

            var previousUndoRedoEnabled = UndoRedoStack.IsEnabled;

            // Disabled undo redo during the node initialization
            UndoRedoStack.IsEnabled = false;

            var node = EditorEnvironment.NodeFactory.CreateNode(catalogItem);

            if(node is IInitializeFromCatalogItem initializeFromCatalogItem)
            {
                initializeFromCatalogItem.Initialize(catalogItem);
            }

            UndoRedoStack.IsEnabled = previousUndoRedoEnabled;

            return node;
        }

        private void InsertNode(ITreeNodeContainer container, int index, ITreeNode node)
        {
            if (node != null && container.TryInsertNode(index, node))
            {
                AddChangeNodeSelectionUndoRedoCommand(node);

                if(node is IReadableNodeContainer containerNode)
                {
                    containerNode.ExpandRecursive();
                }
            }
        }

        private void CreateAndInsertNodeCatalogItem(ITreeNodeContainer container, int index, NodeCatalogItem catalogItem)
        {
            var node = CreateCatalogNode(catalogItem);

            InsertNode(container, index, node);
        }

        private void CreateAndInsertSelectedItem(ITreeNodeContainer container, int index)
        {
            CreateAndInsertNodeCatalogItem(container, index, SelectedCatalogItem);
        }

        private void AddChangeNodeSelectionUndoRedoCommand(ITreeNode newSelection)
        {
            var oldSelection = SelectedNode;

            UndoRedoStack.ExecuteAndPush(new UndoRedoCommand(() => ChangeSelectionCommandAction(newSelection), () => ChangeSelectionCommandAction(oldSelection)));

            void ChangeSelectionCommandAction(ITreeNode commandNode)
            {
                // The previous selection could be null if no item was selected, for example drag & drop from catalog doesn't require a selection
                if (commandNode != null)
                {
                    commandNode.IsSelected = true;
                }
            }
        }

        private ITreeNode CreateNodeCopy(ICopyableNode<ITreeNode> copyable)
        {
            if(copyable == null)
            {
                return null;
            }

            // Disable undo/redo commands during the node copy initialization
            var wasUndoRedoEnabled = UndoRedoStack.IsEnabled;
            UndoRedoStack.IsEnabled = false;

            var copy = copyable.CreateCopy();

            UndoRedoStack.IsEnabled = wasUndoRedoEnabled;

            return copy;
        }

        private ITreeNodeContainer GetDropContainerByLocation(ITreeNode treeNode, DropLocation dropLocation)
        {
            return dropLocation == DropLocation.Inside
                ? treeNode as ITreeNodeContainer
                : treeNode?.Parent as ITreeNodeContainer;
        }

        private (ITreeNodeContainer Container, int Index) GetDropInserInfoByLocation(ITreeNode targetNode, DropLocation dropLocation)
        {
            if (dropLocation == DropLocation.Inside)
            {
                return targetNode is ITreeNodeContainer container
                    ? (container, container.Nodes.Count)
                    : (null, -1);
            }

            if (!(targetNode?.Parent is ITreeNodeContainer parentContainer))
            {
                return (null, -1);
            }

            var nodeIndex = parentContainer.IndexOf(targetNode);

            var indexOffset = dropLocation == DropLocation.Above ? 0 : 1;

            return (parentContainer, nodeIndex + indexOffset);
        }

        bool IContextMenuOpeningHandler.HandleContextMenuOpening()
        {
            _activeContextMenuCommands.Clear();

            foreach(var command in ContextMenuCommands)
            {
                if(!command.CanShow())
                {
                    continue;
                }

                command.UpdateCanExecute();

                _activeContextMenuCommands.Add(command);
            }

            return _activeContextMenuCommands.Count > 0;
        }
    }
}
