using System;
using XFiresecAPI;

namespace Common.GK
{
	public class MRO_2_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x73,
				DriverType = XDriverType.MRO_2,
				UID = new Guid("713702A8-E3A1-4328-9A43-DE9CB5167133"),
				Name = "Модуль речевого оповещения МРО-2М",
				ShortName = "МРО-2М",
				IsControlDevice = true,
				HasLogic = true,
                IsPlaceable = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);

			driver.AvailableCommands.Add(XStateType.TurnOn_InManual);
			driver.AvailableCommands.Add(XStateType.TurnOnNow_InManual);
			driver.AvailableCommands.Add(XStateType.TurnOff_InManual);

			var property1 = new XDriverProperty()
			{
				No = 0x82,
				Name = "Количество повторов",
				Caption = "Количество повторов",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			driver.Properties.Add(property1);

			var property2 = new XDriverProperty()
			{
				No = 0x88,
				Name = "Время отложенного пуска, c",
				Caption = "Время отложенного пуска, c",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			driver.Properties.Add(property2);

			var property3 = new XDriverProperty()
			{
				No = 0x89,
				Name = "Действующее значение входного напряжения линейного входа, mV",
				Caption = "Действующее значение входного напряжения линейного входа, mV",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType
			};
			var property3Parameter1 = new XDriverPropertyParameter()
			{
				Name = "250",
				Value = 0
			};
			var property3Parameter2 = new XDriverPropertyParameter()
			{
				Name = "500",
				Value = 1
			};
			var property3Parameter3 = new XDriverPropertyParameter()
			{
				Name = "775",
				Value = 2
			};
			property3.Parameters.Add(property3Parameter1);
			property3.Parameters.Add(property3Parameter2);
			property3.Parameters.Add(property3Parameter3);
			driver.Properties.Add(property3);

			var property4 = new XDriverProperty()
			{
				No = 0x90,
				Name = "Рабочее напряжение, В",
				Caption = "Рабочее напряжение, В",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType
			};
			var property4Parameter1 = new XDriverPropertyParameter()
			{
				Name = "12",
				Value = 0
			};
			var property4Parameter2 = new XDriverPropertyParameter()
			{
				Name = "24",
				Value = 1
			};
			property4.Parameters.Add(property4Parameter1);
			property4.Parameters.Add(property4Parameter2);
			driver.Properties.Add(property4);

			return driver;
		}
	}
}