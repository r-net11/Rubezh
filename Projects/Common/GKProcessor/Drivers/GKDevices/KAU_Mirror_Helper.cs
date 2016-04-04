using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class KAU_Mirror_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x102,
				DriverType = GKDriverType.GKMirror,
				UID = new Guid("72B08282-457D-4e7b-B86F-EBCC0CC4EF03"),
				Name = "ПМФ-ПФМ",
				ShortName = "ПМФ-ПФМ",
				CanEditAddress = true,
				HasAddress = true,
				IsDeviceOnShleif = false,
				IsRangeEnabled = true,
				MinAddress = 1,
				MaxAddress = 127,
				IsPlaceable = true,
				IsEditMirror = true
			};
			driver.Children.Add(GKDriverType.DetectorDevicesMirror);
			driver.Children.Add(GKDriverType.FireZonesMirror);
			driver.Children.Add(GKDriverType.ControlDevicesMirror);
			driver.Children.Add(GKDriverType.GuardZonesMirror);
			driver.Children.Add(GKDriverType.FirefightingZonesMirror);
			driver.Children.Add(GKDriverType.DirectionsMirror);

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateBits.Add(GKStateBit.No);
			driver.AvailableStateBits.Add(GKStateBit.Norm);
			driver.AvailableStateBits.Add(GKStateBit.Failure);

			var modeProperty = new GKDriverProperty()
			{
				Name = "Mode",
				Caption = "Линия",
				ToolTip = "",
				Default = 0,
				DriverPropertyType = GKDriverPropertyTypeEnum.EnumType,
				IsAUParameter = false
			};
			modeProperty.Parameters.Add(new GKDriverPropertyParameter() { Name = "Основная", Value = 0 });
			modeProperty.Parameters.Add(new GKDriverPropertyParameter() { Name = "Резервная", Value = 1 });
			driver.Properties.Add(modeProperty);


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
					IsAUParameter = true,
					CanNotEdit = true
				}
				);

			driver.Properties.Add(
				new GKDriverProperty()
				{
					No = 1,
					Name = "Parameter 0",
					Caption = "Порог питания, В",
					ToolTip = "",
					Min = 10,
					Max = 240,
					Default = 30,
					DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
					IsAUParameter = true,
					Multiplier = 10,
					CanNotEdit = true
				}
				);

			driver.Properties.Add(
				new GKDriverProperty()
				{
					No = 2,
					Name = "Parameter 1",
					Caption = "Яркость, %",
					ToolTip = "",
					Min = 1,
					Max = 100,
					Default = 100,
					DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
					IsAUParameter = true,
					IsLowByte = true
				}
				);

			return driver;
		}
	}
}