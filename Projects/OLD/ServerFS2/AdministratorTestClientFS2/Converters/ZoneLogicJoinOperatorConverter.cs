using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace AdministratorTestClientFS2.Converters
{
    public class ZoneLogicJoinOperatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((ZoneLogicJoinOperator)value)
            {
                case ZoneLogicJoinOperator.And:
                    return "и";
                case ZoneLogicJoinOperator.Or:
                    return "или";
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
