using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursNetwork.OSI.ApplicationLayer.Devices.ValueConverters
{
    /// <summary>
    /// Преобразует значение (в формате BCD) из массива двух байт 
    /// в значение типа float   
    /// </summary>
    public class BigEndianBcdUInt16ToFloatValueConvertor: IValueConverter
    {
        public ValueType FromArray(byte[] array)
        {
            throw new NotImplementedException();
        }

        public byte[] ToArray(ValueType value)
        {
            throw new NotImplementedException();
        }
    }
}
