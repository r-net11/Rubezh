using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI.ParameterNames
{
    /// <summary>
    /// Иднексы праметров устройтсва Инкотекс Меркурий 203
    /// </summary>
    public enum ParameterNamesMercury203 : int
    {
        Id = 1,
        /// <summary>
        /// Сетевой адрес устройтсва
        /// </summary>
        Address = 2,
        /// <summary>
        /// Групповой адрес устройтсва
        /// </summary>
        GADDR = 3,
        /// <summary>
        /// Дата и время устройства
        /// </summary>
        DateTime = 4,
        /// <summary>
        /// Лимит мощности
        /// </summary>
        PowerLimit = 5,
        /// <summary>
        /// Лимит мощности за месяц
        /// </summary>
        PowerLimitPerMonth = 6
    }
}
