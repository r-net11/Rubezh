using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.Incotex.Models;
using ResursNetwork.BCD;

namespace ResursNetwork.OSI.ApplicationLayer.Devices.ValueConverters
{
    /// <summary>
    /// Конвертер типа DateTime в формат времени и данты счётчкика 
    /// электрической энергии Incotex Меркурий 203
    /// </summary>
    /// <remarks>
    /// Формат времени и даты Incotex Меркурий 203:
    ///     1. Данные хрянятся в BCD - формате
    ///     2. Структура данных dow-hh-mm-ss-dd-mon-yy
    ///         2.1 dow - 1 байт, день недели (0-воскр., 1-пон. … 6-субб.) 7- праздник 0...7h
    ///         2.2 ss - 1 байт, секунды 0...59h
    ///         2.3 mm - 1 байт, минуты 0...59h
    ///         2.4 hh - 1 байт, часы 0...23h
    ///         2.5 dd - 1 байт, день месяца 1...31h
    ///         2.6 mon - 1 байт, месяц (1-январь, 2-февряль и т.д) 1...12h
    ///         2.7 yy - 1 байт, последние две цифры года 0...99h
    /// </remarks>
    public class IncotexDataTimeTypeConverter: IValueConverter
    {
        public ValueType FromArray(byte[] array)
        {
            if (array.Length != 7)
            {
                throw new InvalidCastException(
                    "Невозможно преобразовать массив в IncotexDateTime. Длина массива не равна 7");
            }

            if (!Enum.IsDefined(typeof(Incotex.Models.DayOfWeek), array[0]))
            {
                throw new InvalidCastException(
                    "Невозможно привести значение к типу Incotex.Models.DateTime.DayOfWeek");
            }
            
            return new IncotexDateTime
            {
				DayOfWeek = (Incotex.Models.DayOfWeek)array[0],
                Hours = array[1],
                Minutes = array[2],
                Seconds = array[3],
                DayOfMonth = array[4],
                Month = array[5],
                Year = array[6]
            };
        }

        public byte[] ToArray(ValueType value)
        {
            if (value is IncotexDateTime)
            {
                var dt = (IncotexDateTime)value;

                return new Byte[] 
                {
                    (Byte)dt.DayOfWeek,
                    dt.Hours,
                    dt.Minutes,
                    dt.Seconds,
                    dt.DayOfMonth,
                    dt.Month,
                    dt.Year
                };
            }
            else
            {
                throw new ArgumentException(
                    "Значение имеет недопустимый тип", "value");
            }
        }
    }
}
