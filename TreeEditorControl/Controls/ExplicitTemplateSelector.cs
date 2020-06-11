using System.Windows;
using System.Windows.Controls;

namespace TreeEditorControl.Controls
{
    public class ExplicitTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Template { get; set; }

        public DataTemplate NullTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return NullTemplate;
            }

            return Template;
        }
    }
}
