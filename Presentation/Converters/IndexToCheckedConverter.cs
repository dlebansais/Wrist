using System;
using Windows.UI.Xaml.Data;

namespace Converters
{
    public class IndexToCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return false;

            if (parameter == null)
                return false;

            if (!(parameter is string))
                return false;

            int ExpectedValue;
            if (!int.TryParse(parameter as string, out ExpectedValue))
                return false;

            int IndexValue;

            if (value is int)
                IndexValue = (int)value;
            else if (value is Enum)
                IndexValue = (int)value;
            else if (value is bool)
                IndexValue = (((bool)value) == false) ? 0 : 1;
            else
                return false;

            return IndexValue == ExpectedValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return -1;

            if (parameter == null)
                return -1;

            if (!(parameter is string))
                return -1;

            int ExpectedValue;
            if (!int.TryParse(parameter as string, out ExpectedValue))
                return -1;

            bool IsChecked;

            if (value is bool)
                IsChecked = (bool)value;
            else
                return -1;

            return IsChecked ? ExpectedValue : -1;
        }
    }
}
