using System.Windows;
using System.Windows.Controls;
using TreeEditorControl.Catalog;
using TreeEditorControl.Controls.DragDropHandling;
using TreeEditorControl.Nodes;

namespace TreeEditorControl.Controls
{
    public class CustomCatalogListBox : ListBox
    {
        private DataContextDragHandler<NodeCatalogItem, ITreeNode> _catalogItemDragHandler;

        public CustomCatalogListBox()
        {
            _catalogItemDragHandler = new DataContextDragHandler<NodeCatalogItem, ITreeNode>(this, CustomDragDropFormats.CatalogItemDragDropFormat);

            Loaded += CustomCatalogListBox_Loaded;
            Unloaded += CustomCatalogListBox_Unloaded;
        }

        private void CustomCatalogListBox_Loaded(object sender, RoutedEventArgs e)
        {
            _catalogItemDragHandler.RegisterEvents();
        }

        private void CustomCatalogListBox_Unloaded(object sender, RoutedEventArgs e)
        {
            _catalogItemDragHandler.DeregisterEvents();
        }
    }
}
