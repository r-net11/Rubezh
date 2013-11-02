using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class GKIndicator_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x103,
				DriverType = XDriverType.GKIndicator,
				UID = new Guid("200EED4B-3402-45B4-8122-AE51A4841E18"),
				Name = "Индикатор ГК",
				ShortName = "Индикатор ГК",
				CanEditAddress = false,
                HasAddress = false,
				IsAutoCreate = true,
				MinAddress = 2,
				MaxAddress = 11,
				IsDeviceOnShleif = false,
				IsPlaceable = true,
			};

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);
			driver.AvailableStateBits.Add(XStateBit.On);
			driver.AvailableStateBits.Add(XStateBit.Off);

			var modeProperty = new XDriverProperty()
			{
				No = 0,
				Name = "Mode",
				Caption = "Режим работы",
				ToolTip = "Режим работы индикатора",
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