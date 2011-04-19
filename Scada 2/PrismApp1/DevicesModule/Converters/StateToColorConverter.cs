using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace DevicesModule.Converters
{
    public class StateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string state = (string)value;
            switch (state)
            {
                case "Тревога":
                    return Brushes.Red;

                case "Внимание (предтревожное)":
                    return Brushes.Yellow;

                case "Неисправность":
                    return Brushes.Red;

                case "Требуется обслуживание":
                    return Brushes.Yellow;

                case "Обход устройств":
                    return Brushes.Red;

                case "Неопределено":
                    return Brushes.Gray;

                case "Норма(*)":
                    return Brushes.Blue;

                case "Норма":
                    return Brushes.Green;

                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
