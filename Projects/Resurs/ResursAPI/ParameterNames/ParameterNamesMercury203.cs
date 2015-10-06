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
        public const string GADDR = "GADDR";
        /// <summary>
        /// Дата и время устройства
        /// </summary>
        public const string DateTime = "DateTime";
        /// <summary>
        /// Лимит мощности
        /// </summary>
        public const string PowerLimit = "PowerLimit";
        /// <summary>
        /// Лимит мощности за месяц
        /// </summary>
        public const string PowerLimitPerMonth = "PowerLimitPerMonth";
    }
}
