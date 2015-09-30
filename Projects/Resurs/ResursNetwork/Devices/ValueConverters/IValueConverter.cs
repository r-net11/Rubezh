using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursNetwork.Devices.ValueConverters
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
        Object FromArray(Byte[] array);
        /// <summary>
        /// Преобразует значение параметра в массив байт
        /// </summary>
        /// <returns></returns>
        Byte[] ToArray();
    }
}
