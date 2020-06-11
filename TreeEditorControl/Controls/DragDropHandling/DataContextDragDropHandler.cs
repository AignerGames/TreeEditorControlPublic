using System.Windows.Controls;

using TreeEditorControl.ViewModel;

namespace TreeEditorControl.Controls.DragDropHandling
{
    public abstract class DataContextDragDropHandler<TDrag, TDrop> where TDrag : class where TDrop : class
    {
        public DataContextDragDropHandler(Control control, string dragDropFormat)
        {
            Control = control;
            DragDropFormat = dragDropFormat;
        }

        protected Control Control { get; }

        protected string DragDropFormat { get; }

        protected IDragDropHandler<TDrag, TDrop> DragDropHandler => Control.DataContext as IDragDropHandler<TDrag, TDrop>;

        public abstract void RegisterEvents();

        public abstract void DeregisterEvents();
    }
}
