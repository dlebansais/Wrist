using System;
using System.Diagnostics;
using Windows.UI.Xaml.Data;

namespace Converters
{
    public class IndexToCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                Debug.WriteLine("Called with null value");
                return false;
            }

            if (parameter == null)
            {
                Debug.WriteLine("Called with null parameter");
                return false;
            }

            if (!(parameter is string))
            {
                Debug.WriteLine($"Called with parameter {parameter.GetType()}");
                return false;
            }

            int ExpectedValue;
            if (!int.TryParse(parameter as string, out ExpectedValue))
            {
                Debug.WriteLine($"Called with non-int parameter {parameter}");
                return false;
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
                return false;
            }

            return IndexValue == ExpectedValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                Debug.WriteLine("Called with null value");
                return 0;
            }

            if (parameter == null)
            {
                Debug.WriteLine("Called with null parameter");
                return 0;
            }

            if (!(parameter is string))
            {
                Debug.WriteLine($"Called with parameter {parameter.GetType()}");
                return 0;
            }

            int ExpectedValue;
            if (!int.TryParse(parameter as string, out ExpectedValue))
            {
                Debug.WriteLine($"Called with non-int parameter {parameter}");
                return 0;
            }

            bool IsChecked;

            if (value is bool)
                IsChecked = (bool)value;
            else
            {
                Debug.WriteLine($"Called with {value} and {parameter}");
                return 0;
            }

            return IsChecked ? ExpectedValue : 0;
        }
    }
}
