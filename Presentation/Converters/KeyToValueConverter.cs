using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace Converters
{
    public class KeyToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            Dictionary<string, string> DictionaryValue = value as Dictionary<string, string>;
            if (DictionaryValue == null)
                return null;

            string ParameterAsString = parameter as string;
            if (ParameterAsString == null)
                return null;

            if (DictionaryValue.ContainsKey(ParameterAsString))
                return DictionaryValue[ParameterAsString];
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
