using MsiImageIndexer.helpers;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MsiImageIndexer.converters
{
    public class ImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            return ImageOrientationHelper.OrientImage((Uri)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((BitmapImage)value).UriSource;
        }
    }
}