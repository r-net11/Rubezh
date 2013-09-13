using System;
using XFiresecAPI;

namespace Common.GK
{
	public static class KAU_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x102,
				DriverType = XDriverType.KAU,
				UID = new Guid("4993E06C-85D1-4F20-9887-4C5F67C450E8"),
				Name = "Контроллер адресных устройств",
				ShortName = "КАУ",
				CanEditAddress = true,
				HasAddress = true,
				IsDeviceOnShleif = false,
				IsRangeEnabled = true,
				MinAddress = 1,
				MaxAddress = 127,
				IsPlaceable = true
			};
			driver.AutoCreateChildren.Add(XDriverType.KAUIndicator);

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateBits.Add(XStateBit.No);
			driver.AvailableStateBits.Add(XStateBit.Norm);
			driver.AvailableStateBits.Add(XStateBit.Failure);

			driver.Properties.Add(
				new XDriverProperty()
				{
					No = 0,
					Name = "Parameter 0",
					Caption = "Порог питания, 0.1 В",
					ToolTip = "",
					Min = 100,
					Max = 240,
					Default = 180,
					DriverPropertyType = XDriverPropertyTypeEnum.IntType,
					IsAUParameter = true
				}
				);

			driver.Properties.Add(
				new XDriverProperty()
				{
					No = 1,
					Name = "Parameter 1",
					Caption = "Яркость, %",
					ToolTip = "",
					Min = 0,
					Max = 100,
					Default = 50,
					DriverPropertyType = XDriverPropertyTypeEnum.IntType,
					IsAUParameter = true
				}
				);

			driver.Properties.Add(
				new XDriverProperty()
				{
					No = 2,
					Name = "Parameter 2",
					Caption = "Число неответов адресных устройств",
					ToolTip = "",
					Min = 1,
					Max = 10,
					Default = 5,
					DriverPropertyType = XDriverPropertyTypeEnum.IntType,
					IsAUParameter = true
				}
				);

			driver.Properties.Add(
				new XDriverProperty()
				{
					No = 3,
					Name = "Parameter 3",
					Caption = "Интервал опроса шлейфов, мс",
					ToolTip = "",
					Min = 1000,
					Max = 3000,
					Default = 1000,
					DriverPropertyType = XDriverPropertyTypeEnum.IntType,
					IsAUParameter = true
				}
				);

			var modeProperty = new XDriverProperty()
			{
				Name = "Mode",
				Caption = "Линия",
				ToolTip = "",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
				IsAUParameter = false
			};
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Основная", Value = 0 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Резервная", Value = 1 });
			driver.Properties.Add(modeProperty);

			return driver;
		}
	}
}