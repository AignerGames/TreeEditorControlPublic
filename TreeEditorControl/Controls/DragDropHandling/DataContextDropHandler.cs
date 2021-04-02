using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TreeEditorControl.ViewModel;

namespace TreeEditorControl.Controls.DragDropHandling
{
    public class DataContextDropHandler<TDrag, TDrop> : DataContextDragDropHandler<TDrag, TDrop> where TDrag : class where TDrop : class
    {
        private DropLocation? _currentDropLocation;

        public DataContextDropHandler(Control control, string dragDropFormat) : base(control, dragDropFormat)
        {
        }

        public override void RegisterEvents()
        {
            Control.DragOver += Control_DragOver;
            Control.Drop += Control_Drop;
        }

        public override void DeregisterEvents()
        {
            Control.DragOver -= Control_DragOver;
            Control.Drop -= Control_Drop;
        }

        private void Control_DragOver(object sender, DragEventArgs e)
        {
            if (DragDropHandler == null || !e.TryGetDataContext<TDrop>(out var targetDataContext)
                || !e.TryGetDragDropDataContext<TDrag>(DragDropFormat, out var sourceDataContext))
            {
                return;
            }

            e.Handled = true;

            UpdateDropLocation(e);

            if (_currentDropLocation != null && DragDropHandler.CanDrop(sourceDataContext, targetDataContext, _currentDropLocation.Value))
            {
                e.Effects = DragDropEffects.Move | DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void UpdateDropLocation(DragEventArgs e)
        {
            var targetItem = e.GetSourceParent<CustomTreeViewItem>();
            if (targetItem == null)
            {
                _currentDropLocation = null;
                return;
            }

            // Get the size of the header, because the item returns the hight of the full item, including children
            var itemHeader = (FrameworkElement)targetItem.Template.FindName("PART_Header", targetItem);

            var relativeItemCursorPosition = e.GetPosition(itemHeader);
            var dropPositionY = relativeItemCursorPosition.Y;

            var itemHeight = itemHeader.ActualHeight;
            var itemSegmentSize = itemHeight / 3;

            if (dropPositionY <= itemSegmentSize)
            {
                _currentDropLocation = DropLocation.Above;

                Mouse.SetCursor(Cursors.ScrollN);
            }
            else if (dropPositionY <= (itemSegmentSize * 2))
            {
                _currentDropLocation = DropLocation.Inside;

                Mouse.SetCursor(Cursors.ScrollW);
            }
            else
            {
                _currentDropLocation = DropLocation.Below;

                Mouse.SetCursor(Cursors.ScrollS);
            }
        }

        private void Control_Drop(object sender, DragEventArgs e)
        {
            if (DragDropHandler == null || _currentDropLocation == null  || 
                !e.TryGetDataContext<TDrop>(out var targetDataContext) || 
                !e.TryGetDragDropDataContext<TDrag>(DragDropFormat, out var sourceDataContext))
            {
                return;
            }

            e.Handled = true;

            DragDropHandler.Drop(sourceDataContext, targetDataContext, _currentDropLocation.Value);

            _currentDropLocation = null;
        }
    }
}
