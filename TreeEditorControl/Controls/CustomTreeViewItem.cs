using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace TreeEditorControl.Controls
{
    public class CustomTreeViewItem : TreeViewItem
    {
        public CustomTreeViewItem()
        {
            Loaded += CustomTreeViewItem_Loaded;
            Selected += CustomTreeViewItem_Selected;
        }

        public bool IsMouseInside
        {
            get { return (bool)GetValue(IsMouseInsideProperty); }
            set { SetValue(IsMouseInsideProperty, value); }
        }

        public static readonly DependencyProperty IsMouseInsideProperty = DependencyProperty.Register(
          nameof(IsMouseInside), typeof(bool), typeof(CustomTreeViewItem), new PropertyMetadata());

        private void CustomTreeViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            // The tree (and nodes) are re-loaded if the visible control changes, for example inside a TabControl.
            if(IsSelected)
            {
                // The tree isn't fully loaded yet, so the request has to be handled by the dispatcher, after the tree is ready
                Dispatcher.InvokeAsync(() => 
                {
                    Focus();
                    BringIntoView();
                }, DispatcherPriority.Background);
            }
        }

        private void CustomTreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            // The selected node (IsSelected == true) should always be visible in the view. 
            // For example if the view model sets IsSelected to true after adding / removing nodes.
            Focus();
            BringIntoView();

            e.Handled = true;
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
