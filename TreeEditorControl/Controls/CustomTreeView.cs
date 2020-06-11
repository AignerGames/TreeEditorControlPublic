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
        private DataContextDragHandler<ITreeNode, ITreeNode> _nodeDragHandler;
        private DataContextDropHandler<ITreeNode, ITreeNode> _nodeDropHandler;
        private DataContextDropHandler<NodeCatalogItem, ITreeNode> _catalogItemDropHandler;

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
            // This handles drag over at the tree itself, the other drag/drop handlers are for the nodes
            if(!e.TryGetDataContext<ITreeNode>(out _))
            {
                e.Handled = true;

                e.Effects = DragDropEffects.None;
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
