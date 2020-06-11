using System.Windows;
using System.Windows.Controls;

namespace TreeEditorControl.Controls
{
    public class FallbackTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NullTemplate { get; set; }

        public DataTemplate FallbackTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return NullTemplate;
            }

            // Search a template for the given data type
            if (container is FrameworkElement frameworkElement &&
                frameworkElement.TryFindResource(new DataTemplateKey(item.GetType())) is DataTemplate template)
            {
                return template;
            }

            return FallbackTemplate;
        }
    }
}
