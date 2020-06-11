using System.Windows;
using System.Windows.Controls;

using TreeEditorControl.Nodes;

namespace TreeEditorControl.Controls
{
    public class TreeViewItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NodeTemplate { get; set; }

        public DataTemplate ContainerTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item is IReadableNodeContainer ? ContainerTemplate : NodeTemplate;
        }
    }
}
