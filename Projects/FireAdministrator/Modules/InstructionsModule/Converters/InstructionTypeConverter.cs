using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace InstructionsModule.Converters
{
    public class InstructionTypeConverter : IValueConverter
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
                        return FiresecAPI.Models.EnumsConverter.InstructionTypeToString(instructionType);
                }
                return FiresecAPI.Models.EnumsConverter.InstructionTypeToString(new InstructionType());
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}