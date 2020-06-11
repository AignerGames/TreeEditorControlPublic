using System;
using System.Globalization;

using System.Windows.Data;

namespace TreeEditorControl.Controls
{
    public class IsInstanceOfTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(parameter is Type typeParameter))
            {
                throw new ArgumentException($"Invalid converter parameter, expected 'Type' parameter.");
            }

            return typeParameter.IsInstanceOfType(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
