using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI.ParameterNames
{
    /// <summary>
    /// Иднексы праметров устройтсва Инкотекс Меркурий 203
    /// </summary>
    public enum ParameterNamesMercury203: int
    {
            /// <summary>
            /// Групповой адрес устройтсва
            /// </summary>
            GADDR = 1,
            /// <summary>
            /// Дата и время устройства
            /// </summary>
            DateTime = 2,
            /// <summary>
            /// Лимит мощности
            /// </summary>
            PowerLimit = 3,
            /// <summary>
            /// Лимит мощности за месяц
            /// </summary>
            PowerLimitPerMonth = 4
    }
}
