using System.Windows;
using System.Windows.Controls;


namespace TreeEditorControl.Controls.DragDropHandling
{
    public class DataContextDropHandler<TDrag, TDrop> : DataContextDragDropHandler<TDrag, TDrop> where TDrag : class where TDrop : class
    {
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

            if (DragDropHandler.CanDrop(sourceDataContext, targetDataContext))
            {
                e.Effects = DragDropEffects.Move | DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Control_Drop(object sender, DragEventArgs e)
        {
            if (DragDropHandler == null || !e.TryGetDataContext<TDrop>(out var targetDataContext)
                || !e.TryGetDragDropDataContext<TDrag>(DragDropFormat, out var sourceDataContext))
            {
                return;
            }

            e.Handled = true;

            DragDropHandler.Drop(sourceDataContext, targetDataContext);
        }
    }
}
