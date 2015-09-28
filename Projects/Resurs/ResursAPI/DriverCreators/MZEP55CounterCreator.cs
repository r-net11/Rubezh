using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class MZEP55CounterCreator
	{
		public static Driver Create()
		{
			var driver = new Driver();
			driver.DriverType = DriverType.MZEP55Counter;
			driver.AddParameter("Счётчик подключён", ParameterType.Bool, true);
			driver.AddParameter("Счётчик открыт на чтение", ParameterType.Bool, true);
			driver.AddParameter("Счётчик открыт на запись", ParameterType.Bool, true);
			driver.AddParameter("Пароль первого уровня", ParameterType.String);
			driver.AddParameter("Пароль второго уровня", ParameterType.String);
			driver.AddParameter("Коэффициент трансформации", ParameterType.Double);
			driver.AddParameter("Номер квартиры", ParameterType.Int);
			driver.AddParameter("Почтовый адрес", ParameterType.String);
			driver.AddParameter("Шаг записи расхода в лог", ParameterType.Int);
			driver.AddParameter("Время фиксации расхода для дерева пользователей", ParameterType.Int, true);
			driver.AddParameter("Время фиксации расхода для дерева баланса", ParameterType.Int, true);
			driver.AddParameter("Параметры режимов индикации", ParameterType.Enum, true, new List<ParameterEnumItem>
				{
					new ParameterEnumItem { Name = "В секундах", Value = 0 },
					new ParameterEnumItem { Name = "В минутах", Value = 1 }
				});
			driver.AddParameter("Дата и время", ParameterType.DateTime);
			driver.AddParameter("Ток", ParameterType.Double);
			driver.AddParameter("Напряжение", ParameterType.Double);
			driver.AddParameter("Активная мощность", ParameterType.Double);
			driver.AddParameter("Коэффициент мощности", ParameterType.Double);
			driver.AddParameter("Частота сетевого напряжения", ParameterType.Double);
			driver.AddParameter("Активная энергия по текщему тарифу", ParameterType.Double);
			driver.AddParameter("Время наработки", ParameterType.DateTime);
			driver.AddParameter("Величина ограничения", ParameterType.Double);
			driver.AddParameter("Отображение тарифов", ParameterType.Int);
			return driver;
		}
	}
}
