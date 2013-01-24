using System;
using XFiresecAPI;

namespace Common.GK
{
	public class MDU_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x7E,
				DriverType = XDriverType.MDU,
				UID = new Guid("043fbbe0-8733-4c8d-be0c-e5820dbf7039"),
				Name = "Модуль дымоудаления-1",
				ShortName = "МДУ-1",
				IsControlDevice = true,
				HasLogic = true,
                IsPlaceable = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);

			var property1 = new XDriverProperty()
			{
				No = 0x82,
				Name = "Время переключения электропривода в положение ЗАКРЫТО",
				Caption = "Время переключения электропривода в положение ЗАКРЫТО",
				Default = 180,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			driver.Properties.Add(property1);

			var property2 = new XDriverProperty()
			{
				No = 0x83,
				Name = "Время переключения электропривода в положение ОТКРЫТО",
				Caption = "Время переключения электропривода в положение ОТКРЫТО",
				Default = 180,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			driver.Properties.Add(property2);

			var property3 = new XDriverProperty()
			{
				No = 0x84,
				Name = "Время задержки перед началом движения электропривода в положение ОТКРЫТО",
				Caption = "Время задержки перед началом движения электропривода в положение ОТКРЫТО",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			driver.Properties.Add(property3);

			var property4 = new XDriverProperty()
			{
				No = 0x86,
				Name = "Критическое время без обмена для перехода в защищаемое состояние",
				Caption = "Критическое время без обмена для перехода в защищаемое состояние",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			driver.Properties.Add(property4);

			var property5 = new XDriverProperty()
			{
				No = 0x85,
				Name = "Тип клапана",
				Caption = "Тип клапана",
				Default = 0
			};
			var property5Parameter1 = new XDriverPropertyParameter()
			{
				Name = "Клапан дымоудаления",
				Value = 0
			};
			var property5Parameter2 = new XDriverPropertyParameter()
			{
				Name = "Огнезащитный клапан",
				Value = 1
			};
			property5.Parameters.Add(property5Parameter1);
			property5.Parameters.Add(property5Parameter2);
			driver.Properties.Add(property5);

			var property6 = new XDriverProperty()
			{
				No = 0x85,
				Name = "Тип привода",
				Caption = "Тип привода",
				Default = 0,
				Offset = 0
			};
			var property6Parameter1 = new XDriverPropertyParameter()
			{
				Name = "Реверсивный",
				Value = 0
			};
			var property6Parameter2 = new XDriverPropertyParameter()
			{
				Name = "Пружинный",
				Value = 1
			};
			var property6Parameter3 = new XDriverPropertyParameter()
			{
				Name = "Ручной",
				Value = 2
			};
			property6.Parameters.Add(property6Parameter1);
			property6.Parameters.Add(property6Parameter2);
			property6.Parameters.Add(property6Parameter3);
			driver.Properties.Add(property6);

			var property7 = new XDriverProperty()
			{
				No = 0x86,
				Name = "Перевод заслонки в указанное положение после подачи питания на модуль(Только для пружинного привода)",
				Caption = "Перевод заслонки в указанное положение после подачи питания на модуль(Только для пружинного привода)",
				DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
				Offset = 7
			};
			driver.Properties.Add(property7);

			return driver;
		}
	}
}