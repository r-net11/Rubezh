using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI.ParameterNames
{
	public class ParameterNamesMZEP55Network : ParameterNamesBase
	{
		/// <summary>
		/// Наименование Comport,
		/// тип в БД - string
		/// </summary>
		public const string PortName = "PortName";
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
	}
}
