using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace DevicesModule.Converters
{
    public class ClauseOperationToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ZoneLogicOperation)
            {
                ZoneLogicOperation operation = (ZoneLogicOperation)value;
                switch (operation)
                {
                    case ZoneLogicOperation.All:
                        return "во всех зонах из";

                    case ZoneLogicOperation.Any:
                        return "в любой зонах из";
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}