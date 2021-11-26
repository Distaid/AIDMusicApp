using AIDMusicApp.Models;
using System;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace AIDMusicApp.Converters
{
    public class SongToStringConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return null;

            var name = (string)values[0];
            var guests = values[1] as ObservableCollection<Musician>;

            if (guests != null)
                if (guests.Count != 0)
                {
                    name += " (feat ";
                    name += string.Join(", ", guests.Select((m) => m.Name));
                    name += ")";
                }

            return name;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
