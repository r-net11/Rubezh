using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class BeregunCounterCreator
	{
		public static Driver Create()
		{
			var driver = new Driver();
			driver.DriverType = DriverType.BeregunCounter;
			driver.AddParameter("Счётчик подключён", ParameterType.Bool, true);
			driver.AddParameter("Время фиксации расхода для дерева пользователей", ParameterType.Int, true);
			driver.AddParameter("Время фиксации расхода для дерева баланса", ParameterType.Int, true);
			driver.AddParameter("Шаг записи расхода в лог", ParameterType.Int);
			driver.AddParameter("Коэффициент трансформации", ParameterType.Double);
			driver.AddParameter("Серийный номер", ParameterType.String);
			driver.AddParameter("Последнее время опроса счётчика", ParameterType.DateTime);
			driver.AddParameter("Последнее время ответа счётчика", ParameterType.DateTime);
			return driver;
		}
	}
}
