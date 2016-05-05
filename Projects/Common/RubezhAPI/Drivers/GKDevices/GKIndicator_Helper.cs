using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class GKIndicator_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x103,
				DriverType = GKDriverType.GKIndicator,
				UID = new Guid("200EED4B-3402-45B4-8122-AE51A4841E18"),
				Name = "Индикатор ГК",
				ShortName = "Индикатор ГК",
				HasAddress = false,
				IsAutoCreate = true,
				MinAddress = 2,
				MaxAddress = 11,
				MinAddress2 = 17,
				MaxAddress2 = 22,
				IsDeviceOnShleif = false,
				IsPlaceable = true,
			};

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);
			driver.AvailableStateBits.Add(GKStateBit.On);
			driver.AvailableStateBits.Add(GKStateBit.Off);

			var modeProperty = new GKDriverProperty()
			{
				No = 0,
				Name = "Mode",
				Caption = "Режим работы",
				ToolTip = "Режим работы индикатора",
				Default = 15,
				DriverPropertyType = GKDriverPropertyTypeEnum.EnumType,
				IsAUParameter = true
			};
			modeProperty.Parameters.Add(new GKDriverPropertyParameter() { Name = "Выключено", Value = 0 });
			modeProperty.Parameters.Add(new GKDriverPropertyParameter() { Name = "Мерцает 0.25 с", Value = 1 });
			modeProperty.Parameters.Add(new GKDriverPropertyParameter() { Name = "Мерцает 0.5 с", Value = 3 });
			modeProperty.Parameters.Add(new GKDriverPropertyParameter() { Name = "Мерцает 0.75 с", Value = 7 });
			modeProperty.Parameters.Add(new GKDriverPropertyParameter() { Name = "Включено", Value = 15 });
			driver.Properties.Add(modeProperty);

			return driver;
		}
	}
}