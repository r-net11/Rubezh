using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FiresecAPI.Models;
using System.IO;

namespace InstructionsModule.Converters
{
    class InstructionTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((InstructionType)value)
            {
                case InstructionType.General:
                    return "Общая инструкция";
                case InstructionType.Zone:
                    return "Список зон";
                case InstructionType.Device:
                    return "Список устройств";
                default:
                    return "";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (InstructionType) value;
            //throw new NotImplementedException();
        }

    }
}
