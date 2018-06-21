using System;
using System.Windows.Data;

namespace MoneyVisualizer.Helpers.Ui
{
    public sealed class TwoDecimalPointValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is double) && !(value is decimal))
            {
                return value;
            }

            string valueAsString = value.ToString();
            decimal valueAsDecimal;
            if (!decimal.TryParse(valueAsString, out valueAsDecimal))
            {
                return value;
            }

            return valueAsDecimal.ToString("C");
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
