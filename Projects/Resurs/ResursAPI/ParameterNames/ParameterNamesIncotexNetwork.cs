using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ResursAPI.ParameterNames
{
	public class ParameterNamesIncotexNetwork : ParameterNamesBase
	{
        /// <summary>
		/// Скорость передачи данных,
		/// тип в БД - int
        /// </summary>
		public const string BautRate = "BautRate";
        /// <summary>
        /// Время в течении которого подчинённое устройтсво
		/// должно ответить на запрос мастера, 
		/// тип в БД - int
        /// </summary>
		public const string Timeout = "Timeout";
        /// <summary>
        /// Временная выдержка после предачи широковещательной
		/// комманды, 
		/// тип в БД - int
        /// </summary>
		public const string BroadcastDelay = "BroadcastDelay";
	}
}
