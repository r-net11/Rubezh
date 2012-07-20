using System;
using XFiresecAPI;

namespace Commom.GK
{
	public class GKReleHelper
	{
		public static XDriver Create()
		{
			var xDriver = new XDriver()
			{
				DriverTypeNo = 0x105,
				DriverType = XDriverType.GKRele,
				UID = GKDriversHelper.GKRele_UID,
				OldDriverUID = Guid.Empty,
				CanEditAddress = false,
				HasAddress = true,
				IsAutoCreate = true,
				MinAutoCreateAddress = 14,
				MaxAutoCreateAddress = 15,
				ImageSource = FiresecClient.FileHelper.GetIconFilePath("Device_Device.ico"),
				HasImage = true,
				IsChildAddressReservedRange = false,
				Name = "Реле ГК",
				ShortName = "Реле ГК"
			};

			var modeProperty = new XDriverProperty()
			{
				No = 0,
				Name = "Mode",
				Caption = "Режим работы",
				ToolTip = "Режим работы реле",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
				IsInternalDeviceParameter = true
			};
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Не гореть", Value = 0 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Гореть 0.25 сек", Value = 1 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Гореть 0.5 сек", Value = 3 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Гореть 0.75 сек", Value = 7 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Мигать", Value = 15 });
			xDriver.Properties.Add(modeProperty);

			return xDriver;
		}
	}
}