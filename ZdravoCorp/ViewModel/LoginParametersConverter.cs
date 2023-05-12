using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZdravoCorp.ViewModel
{
    public class LoginParametersConverter : MarkupExtension, IMultiValueConverter
    {
        private static LoginParametersConverter _converter;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new Tuple<string, string>((string)values[0], (string)values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var tuple = (Tuple<string, string>)value;
            return new object[] { tuple.Item1, tuple.Item2 };
        }


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new LoginParametersConverter());
        }
    }
}
