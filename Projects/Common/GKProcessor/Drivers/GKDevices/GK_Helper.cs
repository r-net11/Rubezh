using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class GK_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x102,
				DriverType = XDriverType.GK,
				UID = new Guid("C052395D-043F-4590-A0B8-BC49867ADC6A"),
				Name = "Групповой контроллер",
				ShortName = "ГК",
				IsDeviceOnShleif = false,
				CanEditAddress = false,
				HasAddress = false,
				IsPlaceable = true
			};
			driver.AutoCreateChildren.Add(XDriverType.GKIndicator);
			driver.AutoCreateChildren.Add(XDriverType.GKLine);
			driver.AutoCreateChildren.Add(XDriverType.GKRele);
			driver.Children.Add(XDriverType.KAU);
			driver.Children.Add(XDriverType.RSR2_KAU);

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.Failure);

			driver.Properties.Add(
				new XDriverProperty()
				{
					Name = "IPAddress",
					Caption = "IP адрес",
					ToolTip = "IP адрес",
					DriverPropertyType = XDriverPropertyTypeEnum.StringType,
					IsAUParameter = false
				}
				);

			driver.Properties.Add(
				new XDriverProperty()
				{
					Name = "PollInterval",
					Caption = "Интервал опроса, мс",
					ToolTip = "Интервал опроса, мс",
					DriverPropertyType = XDriverPropertyTypeEnum.IntType,
					Min = 10,
					Max = 1000,
					Default = 1000,
					IsAUParameter = false
				}
				);

			driver.Properties.Add(
				new XDriverProperty()
				{
					No = 0,
					Name = "ConnectionLostCount",
					Caption = "Неответы",
					ToolTip = "Неответы",
					Min = 0,
					Max = 10,
					Default = 3,
					DriverPropertyType = XDriverPropertyTypeEnum.IntType,
					IsAUParameter = true
				}
				);

			driver.Properties.Add(
				new XDriverProperty()
				{
					No = 1,
					Name = "Засыпание, мин",
					Caption = "Засыпание, мин",
					ToolTip = "Засыпание, мин",
					Min = 0,
					Max = 120,
					Default = 10,
					DriverPropertyType = XDriverPropertyTypeEnum.IntType,
					IsAUParameter = true
				}
				);

			driver.Properties.Add(
				new XDriverProperty()
				{
					No = 2,
					Name = "Яркость, %",
					Caption = "Яркость, %",
					ToolTip = "Яркость, %",
					Min = 0,
					Max = 100,
					Default = 50,
					DriverPropertyType = XDriverPropertyTypeEnum.IntType,
					IsAUParameter = true
				}
				);

			return driver;
		}
	}
}