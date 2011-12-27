using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using FiresecAPI.Models;

namespace InstructionsModule.Converters
{
    public class ZonesToStringConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var zones = value as ICollection<ulong?>;
            if (zones.IsNotNullOrEmpty())
            {
                var delimString = ", ";
                var result = new StringBuilder();

                foreach (var zone in zones)
                {
                    result.Append(zone);
                    result.Append(delimString);
                }

                return result.ToString().Remove(result.Length - delimString.Length);
            }

            return null;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}