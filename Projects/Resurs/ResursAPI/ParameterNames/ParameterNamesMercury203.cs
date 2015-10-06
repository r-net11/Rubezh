using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI.ParameterNames
{
    /// <summary>
    /// Иднексы праметров устройтсва Инкотекс Меркурий 203
    /// </summary>
    public class ParameterNamesMercury203 : ParameterNamesBase
    {
        /// <summary>
        /// Групповой адрес устройтсва
        /// </summary>
        public static string GADDR = "GADDR";
        /// <summary>
        /// Дата и время устройства
        /// </summary>
        public static string DateTime = "DateTime";
        /// <summary>
        /// Лимит мощности
        /// </summary>
        public static string PowerLimit = "PowerLimit";
        /// <summary>
        /// Лимит мощности за месяц
        /// </summary>
        public static string PowerLimitPerMonth = "PowerLimitPerMonth";
    }
}
