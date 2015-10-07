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
        /// Наименование Comport
        /// </summary>
		public const string PortName = "PortName";
        /// <summary>
        /// Скорость передачи данных
        /// </summary>
		public const string BautRate = "BautRate";
        /// <summary>
        /// Время в течении которого подчинённое устройтсво
        /// должно ответить на запрос мастера
        /// </summary>
		public const string Timeout = "Timeout";
        /// <summary>
        /// Временная выдержка после предачи широковещательной
        /// комманды
        /// </summary>
		public const string BroadcastDelay = "BroadcastDelay";
	}
}
