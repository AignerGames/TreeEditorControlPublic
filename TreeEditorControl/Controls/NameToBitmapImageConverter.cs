using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TreeEditorControl.Controls
{
    // More info about WPF images https://stackoverflow.com/questions/347614/wpf-image-resources

    /// <summary>
    /// This converter can be used to convert a string to a <see cref="BitmapImage"/>.
    /// The <see cref="BaseUri"/> value must be set, the convert value will be used as image name.
    /// The image extension can be passed as converter parameter or set as <see cref="DefaultImageExtension"/>.
    /// </summary>
    public class NameToBitmapImageConverter : IValueConverter
    {
        private static readonly Dictionary<Uri, BitmapImage> _bitmapImageCache = new Dictionary<Uri, BitmapImage>();

        public Uri BaseUri { get; set; }

        public string DefaultImageExtension { get; set; } = ".png";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is string name))
            {
                return null;
            }

            var imageExtension = parameter is string parameterExtension ? parameterExtension : DefaultImageExtension;

            try
            {
                var uriString = BaseUri + name + imageExtension;
                var bitmapUri = new Uri(uriString, UriKind.Relative);

                if (!_bitmapImageCache.TryGetValue(bitmapUri, out var bitmap))
                {
                    bitmap = new BitmapImage(bitmapUri);

                    _bitmapImageCache[bitmapUri] = bitmap;
                }

                return bitmap;
            }
            catch(Exception ex)
            {
                throw new ArgumentException($"Invalid image path BaseUri: {BaseUri}, Name: {name}, Extension: {imageExtension}", ex);
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
