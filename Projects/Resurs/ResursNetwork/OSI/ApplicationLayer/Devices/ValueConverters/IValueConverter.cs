using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursNetwork.OSI.ApplicationLayer.Devices.ValueConverters
{
    /// <summary>
    /// Реализует преобразование массива байт в значение параметра
    /// и на оборот
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// Преобразует массив в байт в значение параметра
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        ValueType FromArray(Byte[] array);
        /// <summary>
        /// Преобразует значение параметра в массив байт
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Byte[] ToArray(ValueType value);
    }
}
