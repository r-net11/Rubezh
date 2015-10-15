using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI.ParameterNames
{
    public class ParameterNamesBase
    {
        /// <summary>
        /// Идентификатор устройства (Guid)
        /// </summary>
        public const string Id = "Id";
        /// <summary>
        /// Сетевой адрес устройтсва
        /// </summary>
        public const string Address = "Address";
        /// <summary>
        /// Дата и время устройства
        /// </summary>
        public const string DateTime = "DateTime";
		/// <summary>
		/// Наименование Comport,
		/// тип в БД - string
		/// </summary>
		public const string PortName = "PortName";
    }
}
