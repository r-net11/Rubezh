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
			driver.DeviceType = DeviceType.Counter;
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Счётчик подключён",
				ParameterType = ParameterType.Bool,
				IsReadOnly = true,
				Number = 0
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Счётчик открыт на чтение",
				ParameterType = ParameterType.Bool,
				IsReadOnly = true,
				Number = 1
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Счётчик открыт на запись",
				ParameterType = ParameterType.Bool,
				IsReadOnly = true,
				Number = 2
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Пароль первого уровня",
				ParameterType = ParameterType.String,
				Number = 3
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Пароль второго уровня",
				ParameterType = ParameterType.String,
				Number = 4
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Коэффициент трансформации",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0.001,
				DoubleMaxValue = 1000,
				DoubleDefaultValue = 1,
				Number = 5
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Номер квартиры",
				ParameterType = ParameterType.Int,
				IntMinValue = 1,
				IntMaxValue = 1000,
				IntDefaultValue = 1,
				Number = 6,
				IsWriteToDevice = false
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Почтовый адрес",
				ParameterType = ParameterType.String,
				Number = 7,
				IsWriteToDevice = false
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Шаг записи расхода в лог",
				ParameterType = ParameterType.Int,
				IntMinValue = 1,
				IntMaxValue = 1000,
				IntDefaultValue = 5,
				Number = 8
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Дата фиксации расхода для дерева пользователей",
				ParameterType = ParameterType.DateTime,
				IsReadOnly = true,
				Number = 9
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Дата фиксации расхода для дерева баланса",
				ParameterType = ParameterType.DateTime,
				IsReadOnly = true,
				Number = 10
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Дата фиксации расхода для дерева баланса",
				ParameterType = ParameterType.DateTime,
				IsReadOnly = true,
				Number = 11
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Параметры режимов индикации",
				ParameterType = ParameterType.Enum,
				ParameterEnumItems = new List<ParameterEnumItem>
				{
					new ParameterEnumItem { Name = "В секундах", Value = 0 },
					new ParameterEnumItem { Name = "В минутах", Value = 1 }
				},
				EnumDefaultItem = 0,
				Number = 12
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Ток",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0,
				DoubleDefaultValue = 0.5,
				Number = 13
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Напряжение",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0,
				DoubleDefaultValue = 220,
				Number = 14
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Активная мощность",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0,
				DoubleDefaultValue = 1,
				Number = 15
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Коэффициент мощности",
				ParameterType = ParameterType.Double,
				Number = 16
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Частота сетевого напряжения",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0,
				DoubleDefaultValue = 1,
				Number = 17
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Активная энергия по текщему тарифу",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0,
				DoubleDefaultValue = 1,
				Number = 18
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Время наработки",
				ParameterType = ParameterType.DateTime,
				Number = 19
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Величина ограничения",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0,
				DoubleDefaultValue = 1,
				Number = 20
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Отображение тарифов",
				ParameterType = ParameterType.Int,
				IntMinValue = 0,
				IntMaxValue = 8,
				IntDefaultValue = 1,
				Number = 21
			});
			return driver;
		}
	}
}
