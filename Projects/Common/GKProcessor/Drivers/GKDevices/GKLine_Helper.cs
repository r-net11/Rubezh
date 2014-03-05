using System;
using XFiresecAPI;

namespace GKProcessor
{
	public class GKLine_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x104,
				DriverType = XDriverType.GKLine,
				UID = new Guid("DEAA33C2-0EAA-4D4D-BA31-FCDBE0AD149A"),
				Name = "Выход ГК",
				ShortName = "Выход ГК",
				IsControlDevice = true,
				HasLogic = false,
				CanEditAddress = false,
				HasAddress = false,
				//IsAutoCreate = true,
				//MinAddress = 12,
				//MaxAddress = 13,
				IsDeviceOnShleif = false,
				IsPlaceable = true
			};

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);

			driver.AvailableCommandBits.Add(XStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOff_InManual);

			var modeProperty = new XDriverProperty()
			{
				No = 0,
				Name = "Mode",
				Caption = "Режим работы",
				ToolTip = "Режим работы линии",
				Default = 15,
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
				IsAUParameter = true
			};
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Выключено", Value = 0 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Мерцает 0.25 с", Value = 1 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Мерцает 0.5 с", Value = 3 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Мерцает 0.75 с", Value = 7 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Включено", Value = 15 });
			driver.Properties.Add(modeProperty);

			return driver;
		}
	}
}