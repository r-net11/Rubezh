using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursNetwork.BCD
{
    public static class BcdConverter
    {
        #region Methods
        /// <summary>
        /// Проверяет число на соответствие BCD-формату
        /// </summary>
        /// <param name="bcdValue">Число в формате BCD</param>
        /// <returns>Соответствует - true</returns>
        public static Boolean IsValid(Byte bcdValue)
        {
            // Проверяем старшую тетраду байта
            if ((bcdValue >> 4) > 9)
            {
                return false;
            }
            // Проверяем младшую тетраду байта
            if ((bcdValue & 0x0F) > 9)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Преобразует значение в формате BCD в байт
        /// </summary>
        /// <param name="bcdValue"></param>
        /// <returns></returns>
        public static byte ToByte(Byte bcdValue)
        {
            if (IsValid(bcdValue))
            {
                return (byte)(((bcdValue >> 4) * 10) + (bcdValue & 0x0F));
            }
            else
            {
                throw new ArgumentException(
                    "Аргумент не соответствует BCD-формату", "bcdValue");
            }
        }
        /// <summary>
        /// Преобразует значение в значение в формате BCD
        /// </summary>
        /// <param name="value"></param>
        /// <returns>значение в BCD формате</returns>
        public static byte ToBcdByte(Byte value)
        {
            if (value > 99)
            {
                throw new InvalidCastException(
                    "Невозможно преобразовать в BCD формат. Значение слишком большое");
            }
            var high = value / 10;
            var low = value % 10;
            return (Byte)((high << 4) | (low));
        }
        #endregion
    }
}
