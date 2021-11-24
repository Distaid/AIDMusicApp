using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AIDMusicApp.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (parameter.ToString() == "isFormer")
            {
                var val = (bool)value;
                if (val)
                    return "Бывший участник";
                else
                    return "";
            }

            if (parameter.ToString() == "isDead")
            {
                var val = (bool)value;
                if (val)
                    return "Умер";
                else
                    return "";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
