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
			var driver = new Driver(new Guid("0C7DCBA2-A0DF-4481-92ED-CB6198D57233"));
			driver.DriverType = DriverType.BeregunCounter;
			driver.DeviceType = DeviceType.Counter;
			driver.DriverParameters.Add(new DriverParameter(new Guid("B089922B-60A2-4829-ADEA-C5CE4D2B1C81"))
			{
				Name = "Счётчик подключён", 
				Description = "Счётчик подключён", 
				ParameterType = ParameterType.Bool, 
				IsReadOnly = true,
				Number = 0
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("F5D8D913-C8C1-4DD3-B10A-0A9F44BC80FF"))
			{
				Name = "Дата фиксации расхода для дерева пользователей",
				Description = "Дата фиксации расхода для дерева пользователей", 
				ParameterType = ParameterType.DateTime,
				IsReadOnly = true,
				Number = 1
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("EF02A30A-1246-41DD-89A6-C7AE5450B9D3"))
			{
				Name = "Дата фиксации расхода для дерева баланса",
				Description = "Дата фиксации расхода для дерева баланса", 
				ParameterType = ParameterType.DateTime,
				IsReadOnly = true,
				Number = 2
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("35097D6D-F978-46F6-9EB9-29D88C7B87A2"))
			{
				Name = "Шаг записи расхода в лог",
				Description = "Шаг записи расхода в лог",
				ParameterType = ParameterType.Int,
				IntMinValue = 1,
				IntMaxValue = 1000,
				IntDefaultValue = 5,
				Number = 3 
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("910E9E93-AF99-4329-8954-122354F3CE59"))
			{
				Name = "Коэффициент трансформации",
				Description = "Коэффициент трансформации",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0.001,
				DoubleMaxValue = 1000,
				DoubleDefaultValue = 1,
				Number = 4
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("CAA9A194-765B-4B88-9BF5-9B583A569A42"))
			{
				Name = "Серийный номер",
				Description = "Серийный номер",
				ParameterType = ParameterType.String,
				StringDefaultValue = Guid.Empty.ToString(),
				Number = 5
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("884B7059-631D-4BEE-8C99-74DB6B29D2BE"))
			{
				Name = "Последнее время опроса счётчика",
				Description = "Последнее время опроса счётчика",
				ParameterType = ParameterType.DateTime,
				IsReadOnly = true,
				Number = 6
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("B2C369DA-3D0B-42E0-B37B-F5D6B9D6D1BA"))
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
