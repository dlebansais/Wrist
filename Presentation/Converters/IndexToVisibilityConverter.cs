using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Converters
{
    public class IndexToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return Visibility.Collapsed;

            if (parameter == null)
                return Visibility.Collapsed;

            if (!(parameter is string))
                return Visibility.Collapsed;

            int ExpectedValue;
            if (!int.TryParse(parameter as string, out ExpectedValue))
                return Visibility.Collapsed;

            int IndexValue;

            if (value is int)
                IndexValue = (int)value;
            else if (value is Enum)
                IndexValue = (int)value;
            else if (value is bool)
                IndexValue = (((bool)value) == false) ? 0 : 1;
            else
                return Visibility.Collapsed;

            return IndexValue == ExpectedValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
