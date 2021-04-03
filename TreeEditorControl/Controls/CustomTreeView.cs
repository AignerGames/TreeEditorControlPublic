using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using TreeEditorControl.Catalog;
using TreeEditorControl.Controls.DragDropHandling;
using TreeEditorControl.Nodes;
using TreeEditorControl.ViewModel;

namespace TreeEditorControl.Controls
{
    public class CustomTreeViewControl : TreeView
    {
        private readonly DataContextDragHandler<ITreeNode, ITreeNode> _nodeDragHandler;
        private readonly DataContextDropHandler<ITreeNode, ITreeNode> _nodeDropHandler;
        private readonly DataContextDropHandler<NodeCatalogItem, ITreeNode> _catalogItemDropHandler;

        private CustomTreeViewItem _currentTargetItem;

        public CustomTreeViewControl()
        {
            AllowDrop = true;

            _nodeDragHandler = new DataContextDragHandler<ITreeNode, ITreeNode>(this, CustomDragDropFormats.NodeDragDropFormat);
            _nodeDropHandler = new DataContextDropHandler<ITreeNode, ITreeNode>(this, CustomDragDropFormats.NodeDragDropFormat);
            _catalogItemDropHandler = new DataContextDropHandler<NodeCatalogItem, ITreeNode>(this, CustomDragDropFormats.CatalogItemDragDropFormat);

            Loaded += CustomTreeViewControl_Loaded;
            Unloaded += CustomTreeViewControl_Unloaded;
            ContextMenuOpening += CustomTreeViewControl_ContextMenuOpening;
            MouseLeftButtonDown += CustomTreeViewControl_MouseLeftButtonDown;
            DragOver += CustomTreeViewControl_DragOver;
            DragLeave += CustomTreeViewControl_DragLeave;
        }

        private void CustomTreeViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            _nodeDragHandler.RegisterEvents();
            _nodeDropHandler.RegisterEvents();
            _catalogItemDropHandler.RegisterEvents();
        }

        private void CustomTreeViewControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _nodeDragHandler.DeregisterEvents();
            _nodeDropHandler.DeregisterEvents();
            _catalogItemDropHandler.DeregisterEvents();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            UpdateIsMouseInsideState();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            UpdateIsMouseInsideState();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            UpdateIsMouseInsideState();
        }

        private void UpdateIsMouseInsideState()
        {
            // Can't use the normal mouse enter/leave logic, because the events are also called on the parent
            // items, so check if the mouse is really over this item
            var mouseTarget = Mouse.DirectlyOver;
            var mouseItem = mouseTarget.GetParentObject<CustomTreeViewItem>();

            UpdateTargetItem(mouseItem);
        }

        private void CustomTreeViewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Set the focus, otherwise the keyboard shortcuts won't work.
            Focus();
            
            // Important, otherwise other controls will handle the key down and could "steal" the focus
            e.Handled = true;
        }

        private void CustomTreeViewControl_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if(DataContext is IContextMenuOpeningHandler contextMenuOpeningHandler)
            {
                if(!contextMenuOpeningHandler.HandleContextMenuOpening())
                {
                    e.Handled = true;
                }
            }
        }

        private void CustomTreeViewControl_DragOver(object sender, DragEventArgs e)
        {
            // This handles drag over at the tree outside a tree node, before the other drop handlers are called

            var dragTargetItem = e.GetSourceParent<CustomTreeViewItem>();
            UpdateTargetItem(dragTargetItem);

            if(!e.TryGetDataContext<ITreeNode>(out _))
            {
                e.Handled = true;

                e.Effects = DragDropEffects.None;
            }
        }

        private void CustomTreeViewControl_DragLeave(object sender, DragEventArgs e)
        {
            var leaveTarget = e.GetSourceParent<CustomTreeViewItem>();
            if(leaveTarget == null || leaveTarget == _currentTargetItem)
            {
                UpdateTargetItem(null);
            }
        }

        private void UpdateTargetItem(CustomTreeViewItem targetItem)
        {
            if (_currentTargetItem == targetItem)
            {
                return;
            }

            if (_currentTargetItem != null)
            {
                _currentTargetItem.IsMouseInside = false;
            }

            _currentTargetItem = targetItem;

            if (_currentTargetItem != null)
            {
                _currentTargetItem.IsMouseInside = true;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CustomTreeViewItem;
        }
    }
}
