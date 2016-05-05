using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class KAU_RSR2_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x102,
				DriverType = GKDriverType.RSR2_KAU,
				UID = new Guid("57C45124-9300-49BC-A268-68F3D929927B"),
				Name = "Контроллер адресных устройств",
				ShortName = "КАУ",
				CanEditAddress = true,
				HasAddress = true,
				IsDeviceOnShleif = false,
				IsRangeEnabled = true,
				MinAddress = 1,
				MaxAddress = 127,
				IsPlaceable = true,
			};
			driver.AutoCreateChildren.Add(GKDriverType.KAUIndicator);
			driver.AutoCreateChildren.Add(GKDriverType.RSR2_KAU_Shleif);

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
					IsAUParameter = true,
					IsLowByte = true
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

			var als12Property = new GKDriverProperty()
			{
				No = 2,
				IsHieghByte = true,
				Mask = 0x03,
				Name = "als12",
				Caption = "АЛС 1-2",
				ToolTip = "",
				Default = 0,
				DriverPropertyType = GKDriverPropertyTypeEnum.EnumType,
				IsAUParameter = true
			};
			als12Property.Parameters.Add(new GKDriverPropertyParameter() { Name = "Две радиальных", Value = 0 });
			als12Property.Parameters.Add(new GKDriverPropertyParameter() { Name = "Одна кольцевая", Value = 0x03 });
			driver.Properties.Add(als12Property);

			var als34Property = new GKDriverProperty()
			{
				No = 2,
				IsHieghByte = true,
				Mask = 0x0C,
				Name = "als34",
				Caption = "АЛС 3-4",
				ToolTip = "",
				Default = 0,
				DriverPropertyType = GKDriverPropertyTypeEnum.EnumType,
				IsAUParameter = true
			};
			als34Property.Parameters.Add(new GKDriverPropertyParameter() { Name = "Две радиальных", Value = 0 });
			als34Property.Parameters.Add(new GKDriverPropertyParameter() { Name = "Одна кольцевая", Value = 0x0C });
			driver.Properties.Add(als34Property);

			var als56Property = new GKDriverProperty()
			{
				No = 2,
				IsHieghByte = true,
				Mask = 0x30,
				Name = "als56",
				Caption = "АЛС 5-6",
				ToolTip = "",
				Default = 0,
				DriverPropertyType = GKDriverPropertyTypeEnum.EnumType,
				IsAUParameter = true
			};
			als56Property.Parameters.Add(new GKDriverPropertyParameter() { Name = "Две радиальных", Value = 0 });
			als56Property.Parameters.Add(new GKDriverPropertyParameter() { Name = "Одна кольцевая", Value = 0x30 });
			driver.Properties.Add(als56Property);

			var als78Property = new GKDriverProperty()
			{
				No = 2,
				IsHieghByte = true,
				Mask = 0xC0,
				Name = "als78",
				Caption = "АЛС 7-8",
				ToolTip = "",
				Default = 0,
				DriverPropertyType = GKDriverPropertyTypeEnum.EnumType,
				IsAUParameter = true
			};
			als78Property.Parameters.Add(new GKDriverPropertyParameter() { Name = "Две радиальных", Value = 0 });
			als78Property.Parameters.Add(new GKDriverPropertyParameter() { Name = "Одна кольцевая", Value = 0xC0 });
			driver.Properties.Add(als78Property);

			driver.MeasureParameters.Add(new GKMeasureParameter { No = 2, Name = "Питание 1, В", Multiplier = 1024 / 33 });
			driver.MeasureParameters.Add(new GKMeasureParameter { No = 3, Name = "Питание 2, В", Multiplier = 1024 / 33 });
			return driver;
		}
	}
}