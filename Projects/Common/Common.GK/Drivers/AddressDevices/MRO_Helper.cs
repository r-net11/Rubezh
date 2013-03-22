using System;
using XFiresecAPI;

namespace Common.GK
{
	public class MRO_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x74,
				DriverType = XDriverType.MRO,
				UID = new Guid("2d078d43-4d3b-497c-9956-990363d9b19b"),
				Name = "Модуль речевого оповещения МРО-2",
				ShortName = "МРО-2",
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
				Name = "Время отложенного пуска, 5 c",
				Caption = "Время отложенного пуска, 5 c",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255,
				Multiplier = 5
			};
			driver.Properties.Add(property2);

			return driver;
		}
	}
}