using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class MRO_2_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x73,
				DriverType = GKDriverType.MRO_2,
				UID = new Guid("713702A8-E3A1-4328-9A43-DE9CB5167133"),
				Name = "Модуль речевого оповещения МРО-2М",
				ShortName = "МРО-2М",
				IsControlDevice = true,
				HasLogic = true,
				IsPlaceable = true,
				IsIgnored = true,
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);
			
			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);

			var property1 = new GKDriverProperty()
			{
				No = 0x82,
				Name = "Количество повторов",
				Caption = "Количество повторов",
				Default = 3,
				DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			driver.Properties.Add(property1);

			var property2 = new GKDriverProperty()
			{
				No = 0x87,
				Name = "Режим",
				Caption = "Режим",
				Default = 2,
				DriverPropertyType = GKDriverPropertyTypeEnum.EnumType,
				CanNotEdit = true
			};
			var property2Parameter1 = new GKDriverPropertyParameter()
			{
				Name = "Ведомый",
				Value = 1
			};
			var property2Parameter2 = new GKDriverPropertyParameter()
			{
				Name = "Ведущий",
				Value = 2
			};
			property2.Parameters.Add(property2Parameter1);
			property2.Parameters.Add(property2Parameter2);
			driver.Properties.Add(property2);

			var property3 = new GKDriverProperty()
			{
				No = 0x88,
				Name = "Время отложенного пуска, c",
				Caption = "Время отложенного пуска, c",
				Default = 0,
				DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			driver.Properties.Add(property3);

			var property4 = new GKDriverProperty()
			{
				No = 0x89,
				Name = "Действующее значение входного напряжения линейного входа, mV",
				Caption = "Действующее значение входного напряжения линейного входа, mV",
				Default = 2,
				DriverPropertyType = GKDriverPropertyTypeEnum.EnumType
			};
			var property4Parameter1 = new GKDriverPropertyParameter()
			{
				Name = "250",
				Value = 0
			};
			var property4Parameter2 = new GKDriverPropertyParameter()
			{
				Name = "500",
				Value = 1
			};
			var property4Parameter3 = new GKDriverPropertyParameter()
			{
				Name = "775",
				Value = 2
			};
			property4.Parameters.Add(property4Parameter1);
			property4.Parameters.Add(property4Parameter2);
			property4.Parameters.Add(property4Parameter3);
			driver.Properties.Add(property4);

			var property5 = new GKDriverProperty()
			{
				No = 0x90,
				Name = "Рабочее напряжение, В",
				Caption = "Рабочее напряжение, В",
				Default = 0,
				DriverPropertyType = GKDriverPropertyTypeEnum.EnumType
			};
			var property5Parameter1 = new GKDriverPropertyParameter()
			{
				Name = "12",
				Value = 0
			};
			var property5Parameter2 = new GKDriverPropertyParameter()
			{
				Name = "24",
				Value = 1
			};
			property5.Parameters.Add(property5Parameter1);
			property5.Parameters.Add(property5Parameter2);
			driver.Properties.Add(property5);

			return driver;
		}
	}
}