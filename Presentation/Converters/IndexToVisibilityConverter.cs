using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Converters
{
    public class IndexToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                Debug.WriteLine("Called with null value");
                return Visibility.Collapsed;
            }

            if (parameter == null)
            {
                Debug.WriteLine("Called with null parameter");
                return Visibility.Collapsed;
            }

            if (!(parameter is string))
            {
                Debug.WriteLine($"Called with parameter {parameter.GetType()}");
                return Visibility.Collapsed;
            }

            int ExpectedValue;
            if (!int.TryParse(parameter as string, out ExpectedValue))
            {
                Debug.WriteLine($"Called with non-int parameter {parameter}");
                return Visibility.Collapsed;
            }

            int IndexValue;

            if (value is int)
                IndexValue = (int)value;
            else if (value is Enum)
                IndexValue = (int)value;
            else if (value is bool)
                IndexValue = (((bool)value) == false) ? 0 : 1;
            else
            {
                Debug.WriteLine($"Called with {value} and {parameter}");
                return Visibility.Collapsed;
            }

            return IndexValue == ExpectedValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
