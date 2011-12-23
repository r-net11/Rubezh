using System;
using System.Windows.Data;
using FiresecAPI.Models;
using Common;

namespace InstructionsModule.Converters
{
    public class InstructionTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == "All")
            {
                return "Показать все";
            }
            else
            {
                foreach (InstructionType instructionType in Enum.GetValues(typeof(InstructionType)))
                {
                    if ((Enum.GetName(typeof(InstructionType), instructionType) == value.ToString()))
                        return EnumHelper.ToString(instructionType);
                }

                return EnumHelper.ToString(new InstructionType());
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}