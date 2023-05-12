using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace ZdravoCorp.Serializer
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            int intValue = (int)value;
            return intValue.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty((string)value))
                return null;

            int intValue;
            if (int.TryParse((string)value, out intValue))
                return intValue;

            return DependencyProperty.UnsetValue;
        }
    }


}
