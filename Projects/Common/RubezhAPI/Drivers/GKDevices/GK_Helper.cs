using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class GK_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x102,
				DriverType = GKDriverType.GK,
				UID = new Guid("C052395D-043F-4590-A0B8-BC49867ADC6A"),
				Name = "Групповой контроллер",
				ShortName = "ГК",
				IsDeviceOnShleif = false,
				HasAddress = false,
				IsPlaceable = true
			};
			driver.Children.Add(GKDriverType.RSR2_KAU);
			driver.Children.Add(GKDriverType.GKMirror);

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.Failure);

			driver.Properties.Add(
				new GKDriverProperty()
				{
					Name = "IPAddress",
					Caption = "IP адрес",
					ToolTip = "IP адрес",
					DriverPropertyType = GKDriverPropertyTypeEnum.StringType,
					IsAUParameter = false
				}
				);

			driver.Properties.Add(
				new GKDriverProperty()
				{
					Name = "ReservedIPAddress",
					Caption = "Резервный IP адрес",
					ToolTip = "Резервный IP адрес",
					DriverPropertyType = GKDriverPropertyTypeEnum.StringType,
					IsAUParameter = false
				}
				);

			driver.Properties.Add(
				new GKDriverProperty()
				{
					Name = "PollInterval",
					Caption = "Интервал опроса, мс",
					ToolTip = "Интервал опроса, мс",
					DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
					Min = 10,
					Max = 1000,
					Default = 1000,
					IsAUParameter = false
				}
				);

			driver.Properties.Add(
				new GKDriverProperty()
				{
					No = 0,
					Name = "ConnectionLostCount",
					Caption = "Неответы",
					ToolTip = "Неответы",
					Min = 0,
					Max = 10,
					Default = 3,
					DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
					IsAUParameter = true
				}
				);

			driver.Properties.Add(
				new GKDriverProperty()
				{
					No = 1,
					Name = "Засыпание, мин",
					Caption = "Засыпание, мин",
					ToolTip = "Засыпание, мин",
					Min = 0,
					Max = 120,
					Default = 10,
					DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
					IsAUParameter = true
				}
				);

			driver.Properties.Add(
				new GKDriverProperty()
				{
					No = 2,
					Name = "Яркость, %",
					Caption = "Яркость, %",
					ToolTip = "Яркость, %",
					Min = 0,
					Max = 100,
					Default = 50,
					DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
					IsAUParameter = true
				}
				);

			return driver;
		}
	}
}