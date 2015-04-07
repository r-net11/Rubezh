using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class RSR2_MDU24_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xEA,
				DriverType = GKDriverType.RSR2_MDU24,
				UID = new Guid("2C149B68-FFE9-4680-B3CB-BE4CD37BF646"),
				Name = "Модуль автоматики дымоудаления 24В",
				ShortName = "МДУ24-R2",
				IsControlDevice = true,
				HasLogic = true,
				IsPlaceable = true
			};

			driver.AvailableStateBits.Add(GKStateBit.Off);
			driver.AvailableStateBits.Add(GKStateBit.TurningOn);
			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOff);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.Stop_InManual);
			
			var property1 = new GKDriverProperty()
			{
				No = 0,
				Name = "Задержка на включение, с",
				Caption = "Задержка на включение, с",
				Default = 0,
				DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 65535
			};
			driver.Properties.Add(property1);

			var property2 = new GKDriverProperty()
			{
				No = 1,
				Name = "Время включения, с",
				Caption = "Время включения, с",
				Default = 128,
				DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
				Min = 1,
				Max = 65535
			};
			driver.Properties.Add(property2);

			var property3 = new GKDriverProperty()
			{
				No = 2,
				Name = "Время выключения, с",
				Caption = "Время выключения, с",
				Default = 128,
				DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
				Min = 1,
				Max = 65535
			};
			driver.Properties.Add(property3);

			var property4 = new GKDriverProperty()
			{
				No = 3,
				Name = "Тип привода",
				Caption = "Тип привода",
				Default = 0
			};
			var property4Parameter1 = new GKDriverPropertyParameter()
			{
				Name = "Реверсивный",
				Value = 0
			};
			var property4Parameter2 = new GKDriverPropertyParameter()
			{
				Name = "Пружинный",
				Value = 1
			};
			property4.Parameters.Add(property4Parameter1);
			property4.Parameters.Add(property4Parameter2);
			driver.Properties.Add(property4);

			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 3, Name = "Напряжение", Multiplier = 10 });

			return driver;
		}
	}
}
