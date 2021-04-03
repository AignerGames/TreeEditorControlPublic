using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TreeEditorControl.Controls.DragDropHandling
{
    public class DataContextDragHandler<TDrag, TDrop> : DataContextDragDropHandler<TDrag, TDrop> where TDrag : class where TDrop : class
    {
        private Point _dragStartPoint;
        private TDrag _dragDataContext;

        public DataContextDragHandler(Control control, string dragDropFormat) : base(control, dragDropFormat)
        {
        }

        public override void RegisterEvents()
        {
            Control.PreviewMouseLeftButtonDown += Control_PreviewMouseLeftButtonDown;
            Control.PreviewMouseLeftButtonUp += Control_PreviewMouseLeftButtonUp;
            Control.PreviewMouseMove += Control_PreviewMouseMove;

            Control.GiveFeedback += Control_GiveFeedback;
        }

        public override void DeregisterEvents()
        {
            Control.PreviewMouseLeftButtonDown -= Control_PreviewMouseLeftButtonDown;
            Control.PreviewMouseLeftButtonUp -= Control_PreviewMouseLeftButtonUp;
            Control.PreviewMouseMove -= Control_PreviewMouseMove;

            Control.GiveFeedback -= Control_GiveFeedback;
        }

        private void Control_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DragDropHandler != null && e.TryGetDataContext<TDrag>(out var dataContext)
                && DragDropHandler.CanDrag(dataContext))
            {
                _dragStartPoint = e.GetPosition(Control);
                _dragDataContext = dataContext;
            }
        }

        private void Control_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dragDataContext = null;
        }

        private void Control_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragDataContext == null || e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            var point = e.GetPosition(Control);
            var diff = _dragStartPoint - point;

            if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                e.Handled = true;

                var data = new DataObject(DragDropFormat, _dragDataContext);
                DragDrop.DoDragDrop(Control, data, DragDropEffects.Move | DragDropEffects.Copy);

                _dragDataContext = null;
            }
        }

        private void Control_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            // Disable the default cursor if the cursor was set by DragOver
            if(e.Effects == (DragDropEffects.Move | DragDropEffects.Copy))
            {
                e.Handled = true;
            }
        }
    }
}
