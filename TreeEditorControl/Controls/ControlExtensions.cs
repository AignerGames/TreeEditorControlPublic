using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using TreeEditorControl.Nodes;

namespace TreeEditorControl.Controls
{
    internal static class ControlExtensions
    {
        public static bool TryGetDataContext<T>(this RoutedEventArgs eventArgs, out T dataContext) where T : class
        {
            dataContext = (eventArgs.OriginalSource as FrameworkElement)?.DataContext as T;

            return dataContext != null;
        }

        public static bool TryGetDragDropDataContext<T>(this DragEventArgs eventArgs, string format, out T dataContext) where T : class
        {
            dataContext = eventArgs.Data.GetData(format) as T;

            return dataContext != null;
        }

        public static T GetSourceParent<T>(this RoutedEventArgs eventArgs) where T : DependencyObject
        {
            return eventArgs.OriginalSource.GetParentObject<T>();
        }
        public static T GetParentObject<T>(this object obj) where T : DependencyObject
        {
            var currentElement = obj as DependencyObject;
            while (currentElement != null)
            {
                if (currentElement is T correct)
                {
                    return correct;
                }

                currentElement = VisualTreeHelper.GetParent(currentElement);
            }

            return null;
        }

    }
}
