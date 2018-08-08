using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml.Data;

namespace Converters
{
    public class KeyToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                //Debug.WriteLine("Called with null");
                return null;
            }

            Dictionary<string, string> DictionaryValue = value as Dictionary<string, string>;
            if (DictionaryValue == null)
            {
                Debug.WriteLine("Called with " + value.ToString());
                return null;
            }

            string ParameterAsString = parameter as string;
            if (ParameterAsString == null)
            {
                Debug.WriteLine("Called with invalid parameter");
                return null;
            }

            if (DictionaryValue.ContainsKey(ParameterAsString))
                return DictionaryValue[ParameterAsString];
            else
            {
                Debug.WriteLine("Key not found: " + ParameterAsString);
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
