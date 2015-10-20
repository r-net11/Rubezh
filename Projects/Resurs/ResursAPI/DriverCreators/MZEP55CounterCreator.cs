using ResursAPI.ParameterNames;
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
			var driver = new Driver(new Guid("9901A2C5-1B76-419B-86E1-FA8E760BD3B7"));
			driver.DriverType = DriverType.MZEP55Counter;
			driver.DeviceType = DeviceType.Counter;
			driver.CanEditTariffType = false;
			driver.DefaultTariffType = TariffType.Heat;
			driver.DriverParameters.Add(new DriverParameter(new Guid("5AEDF8CD-3527-451A-9760-0F3525A69052"))
			{
				Name = ParameterNamesMZEP55Counter.IsConnected,
				Description = "Счётчик подключён",
				ParameterType = ParameterType.Bool,
				IsReadOnly = true,
				Number = 0
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("20E8C152-7160-4F85-9CD1-870D736F55D7"))
			{
				Name = ParameterNamesMZEP55Counter.CanRead,
				Description = "Счётчик открыт на чтение",
				ParameterType = ParameterType.Bool,
				IsReadOnly = true,
				Number = 1
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("85AB0901-3383-47B0-BD0F-44507E19F6B8"))
			{
				Name = ParameterNamesMZEP55Counter.CanWrite,
				Description = "Счётчик открыт на запись",
				ParameterType = ParameterType.Bool,
				IsReadOnly = true,
				Number = 2
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("F7CEDC4C-F13A-48F1-9764-B2CCC43C87CE"))
			{
				Name = ParameterNamesMZEP55Counter.FirstPassword,
				Description = "Пароль первого уровня",
				ParameterType = ParameterType.String,
				Number = 3
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("090EBACF-4544-49A0-8CB7-288D992D8FFC"))
			{
				Name = ParameterNamesMZEP55Counter.SecondPassword,
				Description = "Пароль второго уровня",
				ParameterType = ParameterType.String,
				Number = 4
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("9608BA97-32C2-4D99-9101-CFE08D789D98"))
			{
				Name = ParameterNamesMZEP55Counter.TransformFactor,
				Description = "Коэффициент трансформации",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0.001,
				DoubleMaxValue = 1000,
				DoubleDefaultValue = 1,
				Number = 5
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("CC8D0223-4710-42CE-BFF2-F52C1C1CF5BB"))
			{
				Name = "Номер квартиры",
				Description = "Номер квартиры",
				ParameterType = ParameterType.Int,
				IntMinValue = 1,
				IntMaxValue = 1000,
				IntDefaultValue = 1,
				Number = 6,
				IsWriteToDevice = false
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("FB76ACCE-292C-4F71-A95C-682DA489657B"))
			{
				Name = "Почтовый адрес",
				Description = "Почтовый адрес",
				ParameterType = ParameterType.String,
				Number = 7,
				IsWriteToDevice = false
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("286F032D-3412-4A46-99D4-9A251A0B452B"))
			{
				Name = ParameterNamesMZEP55Counter.LogStep,
				Description = "Шаг записи расхода в лог",
				ParameterType = ParameterType.Int,
				IntMinValue = 1,
				IntMaxValue = 1000,
				IntDefaultValue = 5,
				Number = 8
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("FD181DE4-A36A-485F-A7C6-D6B269F5F97D"))
			{
				Name = ParameterNamesMZEP55Counter.UserTreeDate,
				Description = "Дата фиксации расхода для дерева пользователей",
				ParameterType = ParameterType.DateTime,
				IsReadOnly = true,
				Number = 9
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("933870E7-2642-4759-95E1-DE61B1334248"))
			{
				Name = ParameterNamesMZEP55Counter.BallanceTreeDate,
				Description = "Дата фиксации расхода для дерева баланса",
				ParameterType = ParameterType.DateTime,
				IsReadOnly = true,
				Number = 10
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("719130BD-620B-4B14-A96A-D3DAD3614CE0"))
			{
				Name = ParameterNamesMZEP55Counter.IndicationParameters,
				Description = "Параметры режимов индикации",
				ParameterType = ParameterType.Enum,
				ParameterEnumItems = new List<ParameterEnumItem>
				{
					new ParameterEnumItem { Name = "В секундах", Value = 0 },
					new ParameterEnumItem { Name = "В минутах", Value = 1 }
				},
				EnumDefaultItem = 0,
				Number = 11
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("ECE08665-A89D-4493-A18F-D3C5DDBD9992"))
			{
				Name = ParameterNamesMZEP55Counter.Current,
				Description = "Ток",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0,
				DoubleDefaultValue = 0.5,
				Number = 12
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("E1A4DA55-B69B-4494-BE64-0065E28B18DE"))
			{
				Name = ParameterNamesMZEP55Counter.Voltage,
				Description = "Напряжение",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0,
				DoubleDefaultValue = 220,
				Number = 13
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("9901A2C5-1B76-419B-86E1-FA8E760BD3B7"))
			{
				Name = ParameterNamesMZEP55Counter.Power,
				Description = "Активная мощность",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0,
				DoubleDefaultValue = 1,
				Number = 14
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("DA0E0D8A-3DC4-4312-8AF0-9D1AB0DF2ED0"))
			{
				Name = ParameterNamesMZEP55Counter.PowerFactor,
				Description = "Коэффициент мощности",
				ParameterType = ParameterType.Double,
				Number = 15
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("7ABBA942-041C-4AF8-974E-4C215BDAE86F"))
			{
				Name = ParameterNamesMZEP55Counter.Frequency,
				Description = "Частота сетевого напряжения",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0,
				DoubleDefaultValue = 1,
				Number = 16
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("7FE656A5-CE1B-4266-B1F5-F71FA5911869"))
			{
				Name = ParameterNamesMZEP55Counter.Energy,
				Description = "Активная энергия по текщему тарифу",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0,
				DoubleDefaultValue = 1,
				Number = 17
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("2DDE3CA7-51A1-48E7-A456-BA72D1B70B60"))
			{
				Name = ParameterNamesMZEP55Counter.WorkoutTime,
				Description = "Время наработки",
				ParameterType = ParameterType.DateTime,
				Number = 18
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("B7A91B17-C584-474A-A6D6-139623030B87"))
			{
				Name = ParameterNamesMZEP55Counter.Restriction,
				Description = "Величина ограничения",
				ParameterType = ParameterType.Double,
				DoubleMinValue = 0,
				DoubleDefaultValue = 1,
				Number = 19
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("92369E98-F920-4155-92B5-D51043E022B3"))
			{
				Name = ParameterNamesMZEP55Counter.ShownTariffsCount,
				Description = "Отображение тарифов",
				ParameterType = ParameterType.Int,
				IntMinValue = 0,
				IntMaxValue = 8,
				IntDefaultValue = 1,
				Number = 20
			});
			return driver;
		}
	}
}
