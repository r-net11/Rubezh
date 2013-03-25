using System;
using XFiresecAPI;

namespace Common.GK
{
	public static class KAU_RSR2_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x102,
				DriverType = XDriverType.RSR2_KAU,
				UID = new Guid("57C45124-9300-49BC-A268-68F3D929927B"),
				Name = "Контроллер адресных устройств RSR2",
				ShortName = "КАУ RSR2",
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
			driver.AvailableStates.Add(XStateType.No);
			driver.AvailableStates.Add(XStateType.Norm);
			driver.AvailableStates.Add(XStateType.Failure);

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
					IsAUParameter = true,
					//Multiplier = 2.8169
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
					IsAUParameter = true,
					IsLowByte = true
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
					IsAUParameter = true,
					IsLowByte = true
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
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Резервная", Value = 3 });
			driver.Properties.Add(modeProperty);

			var als12Property = new XDriverProperty()
			{
				No = 2,
				IsHieghByte = true,
				Mask = 0x03,
				Name = "als12",
				Caption = "АЛС 1-2",
				ToolTip = "",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
				IsAUParameter = true
			};
			als12Property.Parameters.Add(new XDriverPropertyParameter() { Name = "Две радиальных", Value = 0 });
			als12Property.Parameters.Add(new XDriverPropertyParameter() { Name = "Одна кольцевая", Value = 0x03 });
			driver.Properties.Add(als12Property);

			var als34Property = new XDriverProperty()
			{
				No = 2,
				IsHieghByte = true,
				Mask = 0x0C,
				Name = "als34",
				Caption = "АЛС 3-4",
				ToolTip = "",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
				IsAUParameter = true
			};
			als34Property.Parameters.Add(new XDriverPropertyParameter() { Name = "Две радиальных", Value = 0 });
			als34Property.Parameters.Add(new XDriverPropertyParameter() { Name = "Одна кольцевая", Value = 0x0C });
			driver.Properties.Add(als34Property);

			var als56Property = new XDriverProperty()
			{
				No = 2,
				IsHieghByte = true,
				Mask = 0x30,
				Name = "als56",
				Caption = "АЛС 5-6",
				ToolTip = "",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
				IsAUParameter = true
			};
			als56Property.Parameters.Add(new XDriverPropertyParameter() { Name = "Две радиальных", Value = 0 });
			als56Property.Parameters.Add(new XDriverPropertyParameter() { Name = "Одна кольцевая", Value = 0x30 });
			driver.Properties.Add(als56Property);

			var als78Property = new XDriverProperty()
			{
				No = 2,
				IsHieghByte = true,
				Mask = 0xC0,
				Name = "als78",
				Caption = "АЛС 7-8",
				ToolTip = "",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
				IsAUParameter = true
			};
			als78Property.Parameters.Add(new XDriverPropertyParameter() { Name = "Две радиальных", Value = 0 });
			als78Property.Parameters.Add(new XDriverPropertyParameter() { Name = "Одна кольцевая", Value = 0xC0 });
			driver.Properties.Add(als78Property);

			return driver;
		}
	}
}