using System;
using XFiresecAPI;

namespace Commom.GK
{
	public static class GKHelper
	{
		public static XDriver Create()
		{
			var xDriver = new XDriver()
			{
				DriverTypeNo = 0x102,
				DriverType = XDriverType.GK,
				UID = GKDriversHelper.GK_UID,
				OldDriverUID = Guid.Empty,
				CanEditAddress = false,
				HasAddress = false,
				ImageSource = FiresecClient.FileHelper.GetIconFilePath("Device_Device.ico"),
				HasImage = true,
				IsChildAddressReservedRange = false,
				Name = "Групповой контроллер",
				ShortName = "ГК"
			};
			xDriver.AutoCreateChildren.Add(GKDriversHelper.GKIndicator_UID);
			xDriver.AutoCreateChildren.Add(GKDriversHelper.GKLine_UID);
			xDriver.AutoCreateChildren.Add(GKDriversHelper.GKRele_UID);
			xDriver.Children.Add(GKDriversHelper.KAU_UID);

			xDriver.Properties.Add(
				new XDriverProperty()
				{
					Name = "MacAddress1",
					Caption = "Мак адрес 1",
					ToolTip = "Мак адрес 1",
					StringDefault = "1, 2, 3, 4, 5, 6",
					DriverPropertyType = XDriverPropertyTypeEnum.StringType,
					IsInternalDeviceParameter = false
				}
				);

			xDriver.Properties.Add(
				new XDriverProperty()
				{
					Name = "MacAddress2",
					Caption = "Мак адрес 2",
					ToolTip = "Мак адрес 2",
					StringDefault = "2, 2, 2, 2, 2, 2",
					DriverPropertyType = XDriverPropertyTypeEnum.StringType,
					IsInternalDeviceParameter = false
				}
				);

			xDriver.Properties.Add(
				new XDriverProperty()
				{
					Name = "IPAddress1",
					Caption = "IP адрес 1",
					ToolTip = "IP адрес 1",
					StringDefault = "126.1.1.2",
					DriverPropertyType = XDriverPropertyTypeEnum.StringType,
					IsInternalDeviceParameter = false
				}
				);

			xDriver.Properties.Add(
				new XDriverProperty()
				{
					Name = "IPAddress2",
					Caption = "IP адрес 2",
					ToolTip = "IP адрес 2",
					StringDefault = "126.1.1.1",
					DriverPropertyType = XDriverPropertyTypeEnum.StringType,
					IsInternalDeviceParameter = false
				}
				);

			return xDriver;
		}
	}
}