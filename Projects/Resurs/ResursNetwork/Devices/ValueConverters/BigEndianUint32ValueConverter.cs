using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursNetwork.Devices.ValueConverters
{
    /// <summary>
    /// Конвертер значения типа UInt32 в массив из 4 байт
    /// и наоборот. Используется расположение байт от старшего к младшему
    /// (порядок big-endian)
    /// </summary>
    /// <remarks>
    /// https://ru.wikipedia.org/wiki/%D0%9F%D0%BE%D1%80%D1%8F%D0%B4%D0%BE%D0%BA_%D0%B1%D0%B0%D0%B9%D1%82%D0%BE%D0%B2
    /// </remarks>
    public class BigEndianUint32ValueConverter: IValueConverter
    {
        public ValueType FromArray(byte[] array)
        {
            if (array.Length != 4)
            {
                throw new ArgumentOutOfRangeException("array", "Длина массива не равна 4");
            }
            UInt32 value = 0;
            value = ((UInt32)array[0]) << 24;
            value |= ((UInt32)array[1]) << 16;
            value |= ((UInt32)array[2]) << 8;
            value |= array[3];
            return value;
        }

        public byte[] ToArray(ValueType value)
        {
            if (!(value is UInt32))
            {
                throw new InvalidCastException(String.Format(
                    "Невозможно преобразовать значение типа {0}, требуется тип UInt32", 
                    value.GetType()));
            }

            return new Byte[] 
            {
                (Byte)((UInt32)value >> 24),
                (Byte)((UInt32)value >> 16),
                (Byte)((UInt32)value >> 8),
                (Byte)((UInt32)value)
            };
        }
    }
}
