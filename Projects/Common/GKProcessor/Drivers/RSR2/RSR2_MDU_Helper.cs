using System;
using XFiresecAPI;

namespace GKProcessor
{
	public class RSR2_MDU_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xDC,
				DriverType = XDriverType.RSR2_MDU,
				UID = new Guid("1BD3CDB3-7427-4FE8-9B56-22C14C9F5435"),
				Name = "Модуль дымоудаления-1 RSR2",
				ShortName = "МДУ-1 RSR2",
				IsControlDevice = true,
				HasLogic = true,
				IsPlaceable = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOff);
			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			driver.AvailableCommandBits.Add(XStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOff_InManual);
			driver.AvailableCommandBits.Add(XStateBit.Stop_InManual);
			
			var property1 = new XDriverProperty()
			{
				No = 0,
				Name = "Задержка на включение, с",
				Caption = "Задержка на включение, с",
				Default = 10,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 65535
			};
			driver.Properties.Add(property1);

			var property2 = new XDriverProperty()
			{
				No = 1,
				Name = "Время включения, с",
				Caption = "Время включения, с",
				Default = 128,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 1,
				Max = 65535
			};
			driver.Properties.Add(property2);

			var property3 = new XDriverProperty()
			{
				No = 2,
				Name = "Время выключения, с",
				Caption = "Время выключения, с",
				Default = 128,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 1,
				Max = 65535
			};
			driver.Properties.Add(property3);

			var property4 = new XDriverProperty()
			{
				No = 3,
				Name = "Тип привода",
				Caption = "Тип привода",
				Default = 0
			};
			var property4Parameter1 = new XDriverPropertyParameter()
			{
				Name = "Реверсивный",
				Value = 0
			};
			var property4Parameter2 = new XDriverPropertyParameter()
			{
				Name = "Пружинный",
				Value = 1
			};
			property4.Parameters.Add(property4Parameter1);
			property4.Parameters.Add(property4Parameter2);
			driver.Properties.Add(property4);

			driver.MeasureParameters.Add(new XMeasureParameter() { No = 1, Name = "Отсчет задержки, с", IsDelay = true });
			driver.MeasureParameters.Add(new XMeasureParameter() { No = 2, Name = "АЦП концевик ОТКРЫТО" });
			driver.MeasureParameters.Add(new XMeasureParameter() { No = 3, Name = "АЦП концевик ЗАКРЫТО" });
			driver.MeasureParameters.Add(new XMeasureParameter() { No = 4, Name = "АЦП внешняя кнопка НОРМА" });
			driver.MeasureParameters.Add(new XMeasureParameter() { No = 5, Name = "АЦП внешняя кнопка ЗАЩИТА" });

			return driver;
		}
	}
}