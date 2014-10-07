using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class KAU_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x102,
				DriverType = GKDriverType.KAU,
				UID = new Guid("4993E06C-85D1-4F20-9887-4C5F67C450E8"),
				Name = "Контроллер адресных устройств",
				ShortName = "КАУ",
				CanEditAddress = true,
				HasAddress = true,
				IsDeviceOnShleif = false,
				IsRangeEnabled = true,
				MinAddress = 1,
				MaxAddress = 127,
				IsPlaceable = true,
				IsIgnored = true
			};
			driver.AutoCreateChildren.Add(GKDriverType.KAUIndicator);
			driver.AutoCreateChildren.Add(GKDriverType.KAU_Shleif);

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateBits.Add(GKStateBit.No);
			driver.AvailableStateBits.Add(GKStateBit.Norm);
			driver.AvailableStateBits.Add(GKStateBit.Failure);

			driver.Properties.Add(
				new GKDriverProperty()
				{
					No = 0,
					Name = "Parameter 0",
					Caption = "Порог питания, 0.1 В",
					ToolTip = "",
					Min = 100,
					Max = 240,
					Default = 180,
					DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
					IsAUParameter = true
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
					IsAUParameter = true
				}
				);

			driver.Properties.Add(
				new GKDriverProperty()
				{
					No = 2,
					Name = "ConnectionLostCount",
					Caption = "Число неответов адресных устройств",
					ToolTip = "",
					Min = 1,
					Max = 10,
					Default = 5,
					DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
					IsAUParameter = true
				}
				);

			driver.Properties.Add(
				new GKDriverProperty()
				{
					No = 3,
					Name = "Parameter 3",
					Caption = "Интервал опроса шлейфов, мс",
					ToolTip = "",
					Min = 1000,
					Max = 3000,
					Default = 1000,
					DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
					IsAUParameter = true
				}
				);

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

			return driver;
		}
	}
}