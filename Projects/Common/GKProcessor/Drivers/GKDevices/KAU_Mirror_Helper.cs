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
				DriverTypeNo = 0x999,
				DriverType = GKDriverType.RSR2_GKMirror,
				UID = new Guid("72B08282-457D-4e7b-B86F-EBCC0CC4EF03"),
				Name = "ПМФ-ПМЕ",
				ShortName = "ПМФ-ПМЕ",
				CanEditAddress = true,
				HasAddress = true,
				IsDeviceOnShleif = false,
				IsRangeEnabled = true,
				MinAddress = 1,
				MaxAddress = 127,
				IsPlaceable = true,
				IsEditMirror = true
			};
			driver.Children.Add(GKDriverType.RSR2_GKMirrorDetectorsDevice);
			driver.Children.Add(GKDriverType.RSR2_GKMirrorFireZone);
			driver.Children.Add(GKDriverType.RSR2_GKMirrorPerformersDevice);
			driver.Children.Add(GKDriverType.RSR2_GKMirrorGuardZone);
			driver.Children.Add(GKDriverType.RSR2_GKMirrorFightFireZone);
			driver.Children.Add(GKDriverType.RSR2_GKMirrorDirection);

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
					Name = "Parameter 0",
					Caption = "Порог питания, В",
					ToolTip = "",
					Min = 100,
					Max = 240,
					Default = 180,
					DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
					IsAUParameter = true,
					Multiplier = 10,
				}
				);

			driver.Properties.Add(
				new GKDriverProperty()
				{
					No = 1,
					Name = "Parameter 1",
					Caption = "Яркость, %",
					ToolTip = "",
					Min = 1,
					Max = 100,
					Default = 50,
					DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
					IsAUParameter = true,
					IsLowByte = true
				}
				);

			return driver;
		}
	}
}