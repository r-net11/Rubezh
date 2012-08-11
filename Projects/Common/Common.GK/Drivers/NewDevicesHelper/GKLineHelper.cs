using System;
using XFiresecAPI;
using FiresecClient;

namespace Common.GK
{
	public class GKLineHelper
	{
		public static XDriver Create()
		{
			var xDriver = new XDriver()
			{
				DriverTypeNo = 0x104,
				DriverType = XDriverType.GKLine,
				UID = GKDriversHelper.GKLine_UID,
				OldDriverUID = Guid.Empty,
				CanEditAddress = false,
				HasAddress = true,
				IsAutoCreate = true,
				MinAutoCreateAddress = 12,
				MaxAutoCreateAddress = 13,
				HasImage = true,
				IsChildAddressReservedRange = false,
				Name = "Линия ГК",
				ShortName = "Линия ГК"
			};

			var modeProperty = new XDriverProperty()
			{
				No = 0,
				Name = "Mode",
				Caption = "Режим работы",
				ToolTip = "Режим работы линии",
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