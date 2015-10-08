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
			driver.DeviceType = DeviceType.Counter;
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Счётчик подключён", 
				Description = "Счётчик подключён", 
				ParameterType = ParameterType.Bool, 
				IsReadOnly = true,
				Number = 0
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Дата фиксации расхода для дерева пользователей",
				Description = "Дата фиксации расхода для дерева пользователей", 
				ParameterType = ParameterType.DateTime,
				IsReadOnly = true,
				Number = 1
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Дата фиксации расхода для дерева баланса",
				Description = "Дата фиксации расхода для дерева баланса", 
				ParameterType = ParameterType.DateTime,
				IsReadOnly = true,
				Number = 2
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Шаг записи расхода в лог",
				Description = "Шаг записи расхода в лог",
				ParameterType = ParameterType.Int,
				IntMinValue = 1,
				IntMaxValue = 1000,
				IntDefaultValue = 5,
				Number = 3 
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Коэффициент трансформации",
				Description = "Коэффициент трансформации",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0.001,
				DoubleMaxValue = 1000,
				DoubleDefaultValue = 1,
				Number = 4
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Серийный номер",
				Description = "Серийный номер",
				ParameterType = ParameterType.String,
				StringDefaultValue = Guid.Empty.ToString(),
				Number = 5
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Последнее время опроса счётчика",
				Description = "Последнее время опроса счётчика",
				ParameterType = ParameterType.DateTime,
				IsReadOnly = true,
				Number = 6
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Последнее время ответа счётчика",
				Description = "Последнее время ответа счётчика",
				ParameterType = ParameterType.DateTime,
				IsReadOnly = true,
				Number = 7
			});
			return driver;
		}
	}
}
