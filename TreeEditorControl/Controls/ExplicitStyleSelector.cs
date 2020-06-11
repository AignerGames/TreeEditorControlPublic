using System.Windows;
using System.Windows.Controls;

namespace TreeEditorControl.Controls
{
    public class ExplicitStyleSelector : StyleSelector
    {
        public Style Style { get; set; }

        public Style NullStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item == null)
            {
                return NullStyle;
            }

            return Style;
        }
    }
}
