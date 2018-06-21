using System;
using System.Windows;
using System.Windows.Data;

namespace MoneyVisualizer.ValueConverters
{
    /// <summary>
    /// Converts a null object into <seealso cref="Visibility.Hidden"/> while a non-null
    /// object reults <seealso cref="Visibility"/>.
    /// </summary>
    public sealed class NullObjectVisibilityConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null
                ? Visibility.Hidden
                : Visibility.Visible;
            
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
