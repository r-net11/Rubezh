using System;
using System.Windows;
using System.Windows.Data;
using FiresecAPI.Models;

namespace InstructionsModule.Converters
{
    public class StateTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == "All")
            {
                return "Показать все";
            }
            else
            {
                foreach (StateType stateType in Enum.GetValues(typeof(StateType)))
                {
                    if ((Enum.GetName(typeof(StateType), stateType) == value.ToString()))
                    {
                        return FiresecAPI.Models.EnumsConverter.StateTypeToClassName(stateType);
                    }
                }
                return FiresecAPI.Models.EnumsConverter.StateTypeToClassName(new StateType());
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}