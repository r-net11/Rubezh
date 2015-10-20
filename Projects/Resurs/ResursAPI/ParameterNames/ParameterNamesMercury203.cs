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
        /// Групповой адрес устройтсва,
		/// тип в БД - int, только для чтения
        /// </summary>
        public const string GADDR = "GADDR";
        /// <summary>
        /// Лимит мощности,
		/// тип в БД - Double, только для чтения
        /// </summary>
        public const string PowerLimit = "PowerLimit";
        /// <summary>
		/// Лимит мощности за месяц,
		/// тип в БД - Double, только для чтения
        /// </summary>
        public const string PowerLimitPerMonth = "PowerLimitPerMonth";
		/// <summary>
		/// Сождержимое счётчика тарифной зоны 1
		/// </summary>
		public const string CounterTarif1 = "CounterTarif1";
		/// <summary>
		/// Сождержимое счётчика тарифной зоны 1
		/// </summary>
		public const string CounterTarif2 = "CounterTarif2";
		/// <summary>
		/// Сождержимое счётчика тарифной зоны 1
		/// </summary>
		public const string CounterTarif3 = "CounterTarif3";
		/// <summary>
		/// Сождержимое счётчика тарифной зоны 1
		/// </summary>
		public const string CounterTarif4 = "CounterTarif4";
	}
}
