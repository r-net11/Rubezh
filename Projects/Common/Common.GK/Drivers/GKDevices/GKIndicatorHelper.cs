using System;
using XFiresecAPI;

namespace Common.GK
{
	public static class GKIndicatorHelper
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
				HasAddress = true,
				IsAutoCreate = true,
				MinAddress = 2,
				MaxAddress = 11,
				IsDeviceOnShleif = false,
				IsChildAddressReservedRange = false
			};

			var modeProperty = new XDriverProperty()
			{
				No = 0,
				Name = "Mode",
				Caption = "Режим работы",
				ToolTip = "Режим работы индикатора",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
				IsInternalDeviceParameter = true
			};
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Не гореть", Value = 0 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Гореть 0.25 сек", Value = 1 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Гореть 0.5 сек", Value = 3 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Гореть 0.75 сек", Value = 7 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Мигать", Value = 15 });
			driver.Properties.Add(modeProperty);

			return driver;
		}
	}
}