using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace InstructionsModule.Converters
{
    class InstructionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string Value = (string)value;
            if (Value == "All")
            {
                return "Показать все";
            }
            else
            {
                InstructionType instructionType = new InstructionType();
                foreach (InstructionType instrType in Enum.GetValues(typeof(InstructionType)))
                {
                    if ((Enum.GetName(typeof(InstructionType), instrType) == Value))
                    {
                        instructionType = instrType;
                    }
                }
                return FiresecAPI.Models.EnumsConverter.InstructionTypeToString(instructionType);
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

    }
}
